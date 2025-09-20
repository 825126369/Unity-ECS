using System;
using System.Linq;
using System.Xml.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.U2D;

[RequireMatchingQueriesForUpdate]
[BurstCompile]
public partial class PokerAniSystem_Default : SystemBase
{
    private EntityQuery StartDoAniEventQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        StartDoAniEventQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<StartDoAniEvent>().Build(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
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
        if (mInstance.ValueRO.nAniType != PokerAniType.Default)
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
            var animationEntitys = mInstance.ValueRW.animationEntitys;
            for (int i = 0; i < animationEntitys.Length; i++)
            {
                Entity mEntity = animationEntitys[i];
                this.updateAnimation(mEntity, deltaTime);
            }
        }
        else if (mInstance.ValueRO.State == PokerGameState.End)
        {
            mInstance.ValueRW.State = PokerGameState.None;
            UnityMainThreadDispatcher.Instance.Fire(PokerECSEvent.PokerAniFinish);
        }

        foreach (var (mSpriteRenderer, mSpriteRendererCData) in SystemAPI.Query<SystemAPI.ManagedAPI.UnityEngineComponent<UnityEngine.SpriteRenderer>, RefRO<SpriteRendererCData>>())
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
        Sprite spri_bg = atl_game.GetSprite(p_name);
        //n_card.Value.sprite = spri_bg;

        SpriteAtlas atl_game1 = PokerGoMgr.Instance.mPokerBackAtlas;
        string p_name_back = "cardback_1";
        Sprite spri_bg_back = atl_game1.GetSprite(p_name_back);
        //n_back.Value.sprite = spri_bg_back;

        var mEntity_Poker = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Sprite");
        var mEntity_Back = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Back");
        var n_card_cdata = SystemAPI.GetComponentRW<SpriteRendererCData>(mEntity_Poker);
        var n_back_cdata = SystemAPI.GetComponentRW<SpriteRendererCData>(mEntity_Back);
        n_card_cdata.ValueRW.spriteName = p_name;
        n_back_cdata.ValueRW.spriteName = p_name_back;
        n_card_cdata.ValueRW.nOrderId = cardNum;
        this.onSetNormal(mEntity_PokerItem);
    }

    public void UpdatePokerSortingOrderInFly(Entity mEntity_PokerItem)
    {
        var mData = SystemAPI.GetComponentRW<PokerItemCData>(mEntity_PokerItem);
        var mEntity_Poker = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Sprite");
        var mEntity_Back = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Back");

        var n_card_cdata = SystemAPI.GetComponentRW<SpriteRendererCData>(mEntity_Poker);
        var n_back_cdata = SystemAPI.GetComponentRW<SpriteRendererCData>(mEntity_Back);
        n_card_cdata.ValueRW.nOrderId = mData.ValueRO.cardNum + 100;
        n_back_cdata.ValueRW.nOrderId = mData.ValueRO.cardNum + 100;
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
        mInstance.ValueRW.colNodes_Dic = new NativeHashMap<int, NativeList<Entity>>(54, Allocator.Persistent);
        mInstance.ValueRW.allNodes = new NativeList<Entity>(54, Allocator.Persistent);
        mInstance.ValueRW.animationEntitys = new NativeList<Entity>(54, Allocator.Persistent);
        mInstance.ValueRW.colors = colors;
            
        float delay = 0.1f;
        int offsetX = 0;
        for (int i = 0; i < 4; i++)
        {
            int color = colors[i];
            int offset = offsetX * i;
            mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
            Vector3 frompt = mInstance.ValueRO.worldPos_start_list[i];
            delay = i * 0.5f;
            float delayvalue = 0;
            float delayoffset = 0.1f;
            for (int j = 13; j > 0; j--)
            {
                this.showAnimation_Default_ColValue(i, frompt, delay + delayvalue, color, j, offsetX);
                delayvalue += delayoffset;
            }
        }
    }

    void showAnimation_Default_ColValue(int colindex, Vector3 pt, float delay, int color, int value, int offsetX = 0)
    {
        Entity mEntity = this.addStaticCard(pt, color, value, colindex);

        var mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        var mPokerAnimationCData = SystemAPI.GetComponentRW<PokerAnimationCData>(mEntity);
        mPokerAnimationCData.ValueRW.Init(colindex, color, value);
        mPokerAnimationCData.ValueRW.open = true;
        mPokerAnimationCData.ValueRW.trigger = false;
        mPokerAnimationCData.ValueRW.triggerDelay = delay;
        mPokerAnimationCData.ValueRW.btoRight = PokerAnimationCData.toRight(colindex);
        mPokerAnimationCData.ValueRW.startPt = pt;
        mPokerAnimationCData.ValueRW.nowPt = pt;
        mPokerAnimationCData.ValueRW.mEntity = mEntity;

        mPokerAnimationCData.ValueRW.minHeight = mInstance.ValueRO.minHeight;
        mPokerAnimationCData.ValueRW.maxHeight = mInstance.ValueRO.maxHeight;
        mPokerAnimationCData.ValueRW.minWidth = mInstance.ValueRO.minWidth;
        mPokerAnimationCData.ValueRW.maxWidth = mInstance.ValueRO.maxWidth;

        mPokerAnimationCData.ValueRW.vx = PokerAnimationCData.randomVx();
        mPokerAnimationCData.ValueRW.vx_a = 0;
        mPokerAnimationCData.ValueRW.vy = PokerAnimationCData.randomVy();

        if (!mPokerAnimationCData.ValueRW.btoRight)
        {
            mPokerAnimationCData.ValueRW.vx *= -1;
        }

        mPokerAnimationCData.ValueRW.vy_a = PokerAnimationCData.randomVy_a();
        mPokerAnimationCData.ValueRW.deltTime = 0;
        mInstance.ValueRO.animationEntitys.Add(mEntity);
        mInstance.ValueRO.allNodes.Add(mEntity);
    }

    // 检查是否最后一个队列中的最后一个，标志动画结束。   
    Entity addStaticCard(float3 pt, int colorType, int value, int colindex)
    {
        RefRW<PokerSystemSingleton> mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        int nodekey = PokerAnimationCData.getCardId(colorType, value);
        if (!mInstance.ValueRO.colNodes_Dic.TryGetValue(nodekey, out NativeList<Entity> nodeArrs))
        {
            nodeArrs = new NativeList<Entity>(Allocator.Persistent);
            mInstance.ValueRO.colNodes_Dic.Add(nodekey, nodeArrs);
        }

        Entity mTargetEntity = Entity.Null;
        // 从后往前找，找到第一个。
        for (int index = mInstance.ValueRO.animationEntitys.Length - 1; index >= 0; index--)
        {
            var mEntity = mInstance.ValueRO.animationEntitys[index];
            var mPokerAnimationCData2 = EntityManager.GetComponentData<PokerAnimationCData>(mEntity);
            if (mPokerAnimationCData2.color == colorType && mPokerAnimationCData2.value == value && mPokerAnimationCData2.index == colindex)
            {
                mTargetEntity = mEntity;
                break;
            }
        }

        if (nodeArrs.Length >= PokerSystemSingleton.CardsColTotal)
        {
            if (mTargetEntity == null)
            {

            }
            else
            {
                var mPokerAnimationCData2 = EntityManager.GetComponentData<PokerAnimationCData>(mTargetEntity);
                if (mPokerAnimationCData2.color == mInstance.ValueRO.colors[mInstance.ValueRO.colors.Length - 1] && mPokerAnimationCData2.value == 1)
                {
                    this.onAnimatinCallBack();
                    this.DoDestroyAction();
                }
            }
            return Entity.Null;
        }

        Unity.Assertions.Assert.IsTrue(mInstance.ValueRO.Prefab != Entity.Null, "mInstance.Prefab == Entity.Null");
        mTargetEntity = EntityPoolManager.Instance.Spawn(mInstance.ValueRO.Prefab, PoolTagConst.Poker);
        EntityManager.AddComponentData(mTargetEntity, new PokerItemCData());
        EntityManager.AddComponentData(mTargetEntity, new PokerAnimationCData());
        EntityManager.AddComponentData(mTargetEntity, new Parent());

        var mLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(mTargetEntity);
        var mPokerItemCData = SystemAPI.GetComponentRW<PokerItemCData>(mTargetEntity);
        var mParent = SystemAPI.GetComponentRW<Parent>(mTargetEntity);

        mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        mParent.ValueRW.Value = mInstance.ValueRW.cardsNode;
        mLocalTransform.ValueRW.Position = pt;
        mLocalTransform.ValueRW.Scale = mInstance.ValueRW.worldScale_start_list[colindex].x;
        initByNum(mTargetEntity, value, colorType);

        nodeArrs.Add(mTargetEntity);
        return mTargetEntity;
    }

    void updateAnimation(Entity mEntity, float dt)
    {
        var mPokerAnimationCData = SystemAPI.GetComponentRW<PokerAnimationCData>(mEntity);
        var mLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(mEntity);
        if (!mPokerAnimationCData.ValueRW.open)
        {
            return;
        }

        if (mPokerAnimationCData.ValueRW.trigger)
        {
            // 没有节点的时候，不更新。
            if (mEntity == Entity.Null)
            {
                return;
            }

            var deltTime = mPokerAnimationCData.ValueRW.deltTime;
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
                mPokerAnimationCData.ValueRW.maxHeight = mPokerAnimationCData.ValueRW.maxHeight * 0.8f;
            }

            bool willRemove = false;
            if (nowPt.x < mPokerAnimationCData.ValueRW.minWidth - PokerSystemSingleton.CardWidth)
            {
                nowPt.x = mPokerAnimationCData.ValueRW.minWidth;
                willRemove = true;
            }

            if (nowPt.x > mPokerAnimationCData.ValueRW.maxWidth + PokerSystemSingleton.CardWidth)
            {
                nowPt.x = mPokerAnimationCData.ValueRW.maxWidth;
                mPokerAnimationCData.ValueRW.vx *= -1;
                willRemove = true;
            }

            // 每两帧之间 添加
            mPokerAnimationCData.ValueRW.checktimes += 1;
            mLocalTransform.ValueRW.Position = nowPt;
            mPokerAnimationCData.ValueRW.nowPt = nowPt;

            // 碰到边界，不在产生新的额
            if (mPokerAnimationCData.ValueRW.open)
            {
                mLocalTransform.ValueRW.Position = nowPt;
                if (willRemove)
                {
                    mPokerAnimationCData.ValueRW.open = false;
                    if (mPokerAnimationCData.ValueRW.value == 6 && mPokerAnimationCData.ValueRW.index == 2)
                    {
                        this.DoDestroyAction();

                        RefRW<PokerSystemSingleton> mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
                        mInstance.ValueRW.State = PokerGameState.End;
                    }
                    return;
                }
            }
        }
        else
        {
            mPokerAnimationCData.ValueRW.triggerDelay -= dt;
            if (mPokerAnimationCData.ValueRW.triggerDelay <= 0)
            {
                mPokerAnimationCData.ValueRW.trigger = true;
                UpdatePokerSortingOrderInFly(mEntity);

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
        for (int index = 0; index < mInstance.ValueRO.animationEntitys.Length; index++)
        {
            var mEntity = mInstance.ValueRO.animationEntitys[index];
            PokerAnimationCData mPokerAnimationCData = EntityManager.GetComponentData<PokerAnimationCData>(mEntity);
            mPokerAnimationCData.Reset();
        }

        foreach (var v in mInstance.ValueRO.allNodes)
        {
            EntityPoolManager.Instance.Recycle(v);
        }

        //上面回收的时候，有组件增删行为，发生结构性更改
        mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        mInstance.ValueRO.allNodes.Clear();
        mInstance.ValueRO.colNodes_Dic.Clear();
        mInstance.ValueRO.animationEntitys.Clear();
        mInstance.ValueRO.colors.Dispose();
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
