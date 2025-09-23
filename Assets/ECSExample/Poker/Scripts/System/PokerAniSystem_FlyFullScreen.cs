using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.EventSystems.EventTrigger;

[RequireMatchingQueriesForUpdate]
[BurstCompile]
public partial class PokerAniSystem_FlyFullScreen : SystemBase
{
    private EntityQuery StartDoAniEventQuery;
    private NativeList<Entity> mTimerRemoveEntityList;
    private float mFinsihTime = 0;
    protected override void OnCreate()
    {
        base.OnCreate();
        StartDoAniEventQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<StartDoAniEvent>().Build(this);
        mTimerRemoveEntityList = new NativeList<Entity>(5, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        mTimerRemoveEntityList.Dispose();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        //如果系统中途被 Enabled = false，再设为 true，它会再次调用； 类似 MonoBehaviour.Enable
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();
    }

    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        RefRW<PokerSystemSingleton> mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        if(mInstance.ValueRO.nAniType != PokerAniType.FlyFullScreen)
        {
            return;
        }

        if (mInstance.ValueRO.State == PokerGameState.None)
        {
            foreach (var (mEvent, mEntity) in SystemAPI.Query<RefRO<StartDoAniEvent>>().WithEntityAccess())
            {
                mInstance.ValueRW.State = PokerGameState.Start;
                break;
            }

            EntityManager.DestroyEntity(StartDoAniEventQuery);
        }
        else if (mInstance.ValueRO.State == PokerGameState.Start)
        {
            mFinsihTime = 6.0f;
            nOrderId = 0;
            Debug.Log($"PokerAniSystem OnUpdate - Frame: {UnityEngine.Time.frameCount}");
            NativeArray<int> colors = new NativeArray<int>(4, Allocator.Persistent);
            colors[0] = 1;
            colors[1] = 2;
            colors[2] = 3;
            colors[3] = 4;
            Show(colors, null);

            mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
            mInstance.ValueRW.State = PokerGameState.Playing;
        }
        else if (mInstance.ValueRO.State == PokerGameState.Playing)
        {
            float d2 = deltaTime;
            float fixedDeltaTime = 0.01666f;
            while (d2 > 0)
            {
                d2 -= fixedDeltaTime;
                mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
                var allNodes = mInstance.ValueRW.allNodes;
                for (int i = 0; i < allNodes.Length; i++)
                {
                    Entity mEntity = allNodes[i];
                    this.updateAnimation(mEntity, fixedDeltaTime);
                }

                for (int i = mTimerRemoveEntityList.Length - 1; i >= 0; i--)
                {
                    var mEntity = mTimerRemoveEntityList[i];
                    var mPokerTimerRemoveCData = SystemAPI.GetComponentRW<PokerTimerRemoveCData>(mEntity);
                    mPokerTimerRemoveCData.ValueRW.mRomveCdTime -= fixedDeltaTime;
                    if (mPokerTimerRemoveCData.ValueRW.mRomveCdTime <= 0)
                    {
                        EntityManager.RemoveComponent<PokerTimerRemoveCData>(mEntity);
                        mTimerRemoveEntityList.RemoveAt(i);
                        EntityPoolManager.Instance.Recycle(mEntity);
                    }
                }

                mFinsihTime -= fixedDeltaTime;
                if (mFinsihTime <= 0)
                {
                    DoDestroyAction();
                    mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
                    mInstance.ValueRW.State = PokerGameState.End;
                }
            }
        }
        else if (mInstance.ValueRO.State == PokerGameState.End)
        {
            mInstance.ValueRW.State = PokerGameState.None;
            UnityMainThreadDispatcher.Instance.Fire(PokerECSEvent.PokerAniFinish);
        }

        foreach (var (mSpriteRenderer, mSpriteRendererCData) in 
            SystemAPI.Query<SystemAPI.ManagedAPI.UnityEngineComponent<UnityEngine.SpriteRenderer>, 
            RefRO<SpriteRendererCData>>())
        {
            SpriteAtlas mSpriteAtlas = PokerGoMgr.Instance.mPokerAtlas;
            Sprite spri_bg = mSpriteAtlas.GetSprite(mSpriteRendererCData.ValueRO.spriteName.ToString());
            mSpriteRenderer.Value.sprite = spri_bg;
            mSpriteRenderer.Value.sortingOrder = mSpriteRendererCData.ValueRO.nOrderId;
        }
    }
    
    public void initByNum(Entity mEntity_PokerItem, int cardNum, int colorType)
    {
        var mData = SystemAPI.GetComponentRW<PokerItemCData>(mEntity_PokerItem);
        mData.ValueRW.color = colorType;
        mData.ValueRW.cardNum = cardNum;
        mData.ValueRW.nCardId = colorType * 13 + cardNum;

        //刷新花色点数ui
        SpriteAtlas atl_game = PokerGoMgr.Instance.mPokerAtlas;
        string p_name = "di_" + cardNum + "_" + colorType;
        SpriteAtlas atl_game1 = PokerGoMgr.Instance.mPokerBackAtlas;
        string p_name_back = "cardback_1";

        var mEntity_Poker = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Sprite");
        var mEntity_Back = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Back");
        var n_card_cdata = SystemAPI.GetComponentRW<SpriteRendererCData>(mEntity_Poker);
        var n_back_cdata = SystemAPI.GetComponentRW<SpriteRendererCData>(mEntity_Back);
        n_card_cdata.ValueRW.spriteName = p_name;
        n_back_cdata.ValueRW.spriteName = p_name_back;
        n_card_cdata.ValueRW.nOrderId = cardNum;
        this.onSetNormal(mEntity_PokerItem);
    }

    int nOrderId = 0;
    public void UpdatePokerSortingOrderInFly(Entity mEntity_PokerItem)
    {
        var mData = SystemAPI.GetComponentRW<PokerItemCData>(mEntity_PokerItem);
        var mEntity_Poker = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Sprite");
        var mEntity_Back = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Back");

        var n_card_cdata = SystemAPI.GetComponentRW<SpriteRendererCData>(mEntity_Poker);
        var n_back_cdata = SystemAPI.GetComponentRW<SpriteRendererCData>(mEntity_Back);
        Debug.Log("nOrderId: " + nOrderId);
        n_card_cdata.ValueRW.nOrderId = nOrderId++ + 100;
        this.onSetNormal(mEntity_PokerItem);
    }
    
    void onSetBack(Entity mEntity_PokerItem)
    {
        //mData.n_card.Value.gameObject.SetActive(false);
        //mData.n_back.Value.gameObject.SetActive(true);
    }

    void onSetNormal(Entity mEntity_PokerItem)
    {
        //mData.n_back.Value.gameObject.SetActive(false);
        //mData.n_card.Value.gameObject.SetActive(true);
    }

    public void Show(NativeArray<int> colors, Action callback)
    {
        RefRW<PokerSystemSingleton> mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        mInstance.ValueRW.animationOver = false;
        mInstance.ValueRW.allNodes = new NativeList<Entity>(54, Allocator.Persistent);
        mInstance.ValueRW.colors = colors;

        int nAniIndex = 0;
        int nHengIndex = 0;
        int offsetX = 0;
        for (int i = 0; i < 4; i++)
        {
            int color = colors[i];
            int offset = offsetX * i;
            mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
            Vector3 frompt = mInstance.ValueRO.worldPos_start_list[i];

            int nShuIndex = 0;
            for (int j = 13; j > 0; j--)
            {
                float delay = nShuIndex * 0.7f + nHengIndex * 0.18f;
                this.showAnimation_Default_ColValue(i, frompt, delay, color, j, offsetX);
                nAniIndex++;
                nShuIndex++;
            }
            nHengIndex++;
        }
    }

    void showAnimation_Default_ColValue(int colindex, Vector3 pt, float delay, int color, int value, int offsetX = 0)
    {
        Entity mEntity = this.addStaticCard(pt, color, value);

        var mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        var mPokerAnimationCData = SystemAPI.GetComponentRW<PokerAnimationCData2>(mEntity);
        var mLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(mEntity);

        mPokerAnimationCData.ValueRW.Init(colindex, color, value);
        mPokerAnimationCData.ValueRW.open = true;
        mPokerAnimationCData.ValueRW.trigger = false;
        mPokerAnimationCData.ValueRW.triggerDelay = delay;
        mPokerAnimationCData.ValueRW.btoRight = PokerAnimationCData2.toRight();
        mPokerAnimationCData.ValueRW.startPt = pt;
        mPokerAnimationCData.ValueRW.nowPt = pt;
        mPokerAnimationCData.ValueRW.mEntity = mEntity;

        mPokerAnimationCData.ValueRW.minHeight = mInstance.ValueRO.minHeight;
        mPokerAnimationCData.ValueRW.maxHeight = mInstance.ValueRO.maxHeight;
        mPokerAnimationCData.ValueRW.minWidth = mInstance.ValueRO.minWidth;
        mPokerAnimationCData.ValueRW.maxWidth = mInstance.ValueRO.maxWidth;

        mPokerAnimationCData.ValueRW.vx = PokerAnimationCData2.randomVx();
        mPokerAnimationCData.ValueRW.vx_a = 0;
        mPokerAnimationCData.ValueRW.vy = PokerAnimationCData2.randomVy();

        if (!mPokerAnimationCData.ValueRW.btoRight)
        {
            mPokerAnimationCData.ValueRW.vx *= -1;
        }

        mPokerAnimationCData.ValueRW.vy_a = PokerAnimationCData2.randomVy_a();
        mInstance.ValueRW.allNodes.Add(mEntity);
    }

    // 检查是否最后一个队列中的最后一个，标志动画结束。   
    Entity addStaticCard(float3 pt, int colorType, int value)
    {
        RefRW<PokerSystemSingleton> mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();

        Unity.Assertions.Assert.IsTrue(mInstance.ValueRO.Prefab != Entity.Null, "mInstance.Prefab == Entity.Null");
        Entity mTargetEntity = EntityPoolManager.Instance.Spawn(mInstance.ValueRO.Prefab, PoolTagConst.Poker);
        ECSHelper.AddMissComponentData<PokerItemCData>(EntityManager, mTargetEntity);
        ECSHelper.AddMissComponentData<PokerAnimationCData2>(EntityManager, mTargetEntity);
        ECSHelper.AddMissComponentData<Parent>(EntityManager, mTargetEntity);

        var mLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(mTargetEntity);
        var mPokerItemCData = SystemAPI.GetComponentRW<PokerItemCData>(mTargetEntity);
        var mParent = SystemAPI.GetComponentRW<Parent>(mTargetEntity);
        mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        mParent.ValueRW.Value = mInstance.ValueRW.cardsNode;
        mLocalTransform.ValueRW.Position = pt;
        mLocalTransform.ValueRW.Scale = mInstance.ValueRW.worldScale_start_list[0].x;
        initByNum(mTargetEntity, value, colorType);
        return mTargetEntity;
    }

    private void CloneFlyObj(Entity mEntry)
    {
        var mPokerAnimationCData = SystemAPI.GetComponentRO<PokerAnimationCData2>(mEntry);
        var mLocalTransform = SystemAPI.GetComponentRO<LocalTransform>(mEntry);
        Entity mObj = addStaticCard(mLocalTransform.ValueRO.Position, 
            mPokerAnimationCData.ValueRO.color, 
            mPokerAnimationCData.ValueRO.value);

        UpdatePokerSortingOrderInFly(mObj);
        mTimerRemoveEntityList.Add(mObj);
        EntityManager.AddComponentData(mObj, new PokerTimerRemoveCData() { mRomveCdTime = 6.0f});
    }

    void updateAnimation(Entity mEntity, float dt)
    {
        var mPokerAnimationCData = SystemAPI.GetComponentRW<PokerAnimationCData2>(mEntity);
        if (!mPokerAnimationCData.ValueRW.open)
        {
            return;
        }

        mPokerAnimationCData = SystemAPI.GetComponentRW<PokerAnimationCData2>(mEntity);
        var mLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(mEntity);
        if (mPokerAnimationCData.ValueRW.trigger)
        {
            var startPt = mPokerAnimationCData.ValueRW.nowPt;
            var maxHeight = mPokerAnimationCData.ValueRW.maxHeight;
            var toRight = mPokerAnimationCData.ValueRW.btoRight;
            var vx_a = mPokerAnimationCData.ValueRW.vx_a;
            var vy_a = mPokerAnimationCData.ValueRW.vy_a;
            var nowPt = new Vector3(0, 0, 0);
            // 匀变速直线运动位移公式：a=dv/dt，
            // 距离 x = v0t+1/2·at^2

            // 现在速度
            mPokerAnimationCData.ValueRW.vx += vx_a * dt;
            mPokerAnimationCData.ValueRW.vy += vy_a * dt;
            var vx = mPokerAnimationCData.ValueRW.vx;
            var vy = mPokerAnimationCData.ValueRW.vy;

            nowPt.x = (float)(startPt.x + vx * dt + 0.5f * vx_a * dt * dt);
            nowPt.y = (float)(startPt.y + vy * dt + 0.5f * vy_a * dt * dt);
            nowPt.z = startPt.z;

            // 垂直. 小于最低值。
            if (nowPt.y < mPokerAnimationCData.ValueRW.minHeight)
            {
                nowPt.y = mPokerAnimationCData.ValueRW.minHeight;
                mPokerAnimationCData.ValueRW.vy *= -0.95f;  //转变方向
            }

            if (nowPt.y > mPokerAnimationCData.ValueRW.maxHeight)
            {
                nowPt.y = mPokerAnimationCData.ValueRW.maxHeight;
                mPokerAnimationCData.ValueRW.vy = 0;
                mPokerAnimationCData.ValueRW.maxHeight = mPokerAnimationCData.ValueRW.maxHeight * 0.7f;
            }

            // 每两帧之间 添加
            mPokerAnimationCData.ValueRW.checktimes += 1;
            mLocalTransform.ValueRW.Position = nowPt;
            mPokerAnimationCData.ValueRW.nowPt = nowPt;
            
            CloneFlyObj(mEntity);
            mPokerAnimationCData = SystemAPI.GetComponentRW<PokerAnimationCData2>(mEntity);
            if (Mathf.Abs(nowPt.x) > mPokerAnimationCData.ValueRW.maxWidth + 100)
            {
                mPokerAnimationCData.ValueRW.open = false;
            }
        }
        else
        {
            mPokerAnimationCData.ValueRW.triggerDelay -= dt;
            if (mPokerAnimationCData.ValueRW.triggerDelay <= 0)
            {
                mPokerAnimationCData.ValueRW.trigger = true;
                mLocalTransform.ValueRW.Scale = 0;
            }
        }
    }

    void onAnimatinCallBack()
    {
        PokerSystemSingleton mInstance = SystemAPI.GetSingleton<PokerSystemSingleton>();
        mInstance.State = PokerGameState.End;

        //if (this.callBack != null)
        //{
        //    this.callBack();
        //    this.callBack = null;
        //}
    }

    public void DoDestroyAction()
    {
        RefRW<PokerSystemSingleton> mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        if (mInstance.ValueRO.animationOver)
        {
            return;
        }

        mInstance.ValueRW.animationOver = true;
        for (int index = 0; index < mInstance.ValueRO.allNodes.Length; index++)
        {
            var mEntity = mInstance.ValueRO.allNodes[index];
            PokerAnimationCData2 mPokerAnimationCData = EntityManager.GetComponentData<PokerAnimationCData2>(mEntity);
            mPokerAnimationCData.Reset();
        }

        foreach (var v in mInstance.ValueRO.allNodes)
        {
            EntityPoolManager.Instance.Recycle(v);
        }

        //上面回收的时候，有组件增删行为，发生结构性更改
        mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        mInstance.ValueRO.allNodes.Clear();
        mInstance.ValueRO.colors.Dispose();

        foreach (var v in mTimerRemoveEntityList)
        {
            var mEntity = v;
            EntityManager.RemoveComponent<PokerTimerRemoveCData>(mEntity);
            EntityPoolManager.Instance.Recycle(mEntity);
        }
        mTimerRemoveEntityList.Clear();
    }

    public void onClick_Skip()
    {
        //this.skipNode.SetActive(false);
        //AudioController.Instance.playSound(Sounds.button, 1);
        //this.onAnimatinCallBack();
        //this.DoDestroyAction();
        //TAController.Instance.trackAnimationSkip();
    }
}
