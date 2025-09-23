using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.U2D;

[RequireMatchingQueriesForUpdate]
[BurstCompile]
public partial class PokerAniSystem_FlyFullScreen3 : SystemBase
{
    private float mFinsihTime = 0;

    public partial struct SetPokerItemDataEvent : IComponentData
    {

    }

    public partial struct SetPokerItemSortingOrderEvent : IComponentData
    {

    }

    protected override void OnCreate()
    {
        base.OnCreate();
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

        if(!SystemAPI.HasSingleton<PokerSystemSingleton>())
        {
            return;
        }

        RefRW<PokerSystemSingleton> mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        if(mInstance.ValueRO.nAniType != PokerAniType.FlyFullScreen3)
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

            var StartDoAniEventQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<StartDoAniEvent>().Build(this);
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
            foreach (var (mEvent, mPokerItemCData, mEntity) in SystemAPI.Query<RefRO<SetPokerItemDataEvent>, RefRO<PokerItemCData>>().WithEntityAccess())
            {
                SetGameObjectSprite(mEntity, mPokerItemCData.ValueRO);
            }

            foreach (var (mEvent, mPokerItemCData, mEntity) in SystemAPI.Query<RefRO<SetPokerItemSortingOrderEvent>, RefRO<PokerItemCData>>().WithEntityAccess())
            {
                UpdatePokerSortingOrderInFly(mEntity);
            }

            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                              .CreateCommandBuffer(World.Unmanaged);
            
            var mJob1 = new UpdateCardAnimationJob()
            {
                DeltaTime = deltaTime,
                ECB = ecb.AsParallelWriter(),
                Prefab = mInstance.ValueRO.Prefab,
                Parent = mInstance.ValueRO.cardsNode,
                OriScale = mInstance.ValueRO.worldScale_start_list[0]
            };
            this.Dependency = mJob1.ScheduleParallel(GetEntityQuery(typeof(PokerAnimationCData2), typeof(LocalTransform)), this.Dependency);
            this.Dependency.Complete();
            var mJob2 = new TimerRemoveJob()
            {
                DeltaTime = deltaTime,
                ECB = ecb.AsParallelWriter(),
            };
            this.Dependency = mJob2.ScheduleParallel(GetEntityQuery(typeof(PokerTimerRemoveCData), typeof(LocalTransform)), this.Dependency);
            this.Dependency.Complete();

            EntityCommandBuffer ecb2 = new EntityCommandBuffer(Allocator.Temp);
            ecb2.RemoveComponent(GetEntityQuery(typeof(SetPokerItemDataEvent)), typeof(SetPokerItemDataEvent), EntityQueryCaptureMode.AtPlayback);
            ecb2.RemoveComponent(GetEntityQuery(typeof(SetPokerItemSortingOrderEvent)), typeof(SetPokerItemSortingOrderEvent), EntityQueryCaptureMode.AtPlayback);
            ecb2.Playback(EntityManager);

            mFinsihTime -= deltaTime;
            if (mFinsihTime <= 0)
            {
                mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
                mInstance.ValueRW.State = PokerGameState.End;
            }
        }
        else if (mInstance.ValueRO.State == PokerGameState.End)
        {
            mInstance.ValueRW.State = PokerGameState.None;
            UnityMainThreadDispatcher.Instance.Fire(PokerECSEvent.PokerAniFinish);

            EntityCommandBuffer ecb2 = new EntityCommandBuffer(Allocator.Temp);
            var mEntityList = GetEntityQuery(typeof(PokerItemCData)).ToEntityArray(Allocator.Temp);//这里的查询 由于是下一帧，缓存更新了
            foreach (var v in mEntityList)
            {
                ecb2.DestroyEntity(v);
            }

            ecb2.Playback(EntityManager);
        }
    }

    [BurstCompile]
    partial struct TimerRemoveJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        void Execute(
            [EntityIndexInQuery] int entityIndexInQuery,
            ref PokerTimerRemoveCData mPokerTimerRemoveCData,
            Entity mEntity)
        {
            mPokerTimerRemoveCData.mRomveCdTime -= DeltaTime;
            if (mPokerTimerRemoveCData.mRomveCdTime <= 0)
            {
                ECB.DestroyEntity(entityIndexInQuery, mEntity);
            }
        }
    }

    [BurstCompile]
    partial struct UpdateCardAnimationJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        public Entity Prefab;
        public Entity Parent;
        public float3 OriScale;

        void Execute(
            [EntityIndexInQuery] int entityIndexInQuery,
            ref PokerAnimationCData2 mPokerAnimationCData,
            ref LocalTransform mLocalTransform,
            Entity mEntity)
        {
            float d2 = DeltaTime;
            float fixedDeltaTime = 0.01666f;
            while (d2 > 0)
            {
                d2 -= fixedDeltaTime;
                updateAnimation(
                    fixedDeltaTime,
                    entityIndexInQuery,
                    ref mPokerAnimationCData,
                    ref mLocalTransform,
                    ref mEntity);
            }
        }

        void updateAnimation(
            float dt,
            int entityIndexInQuery,
            ref PokerAnimationCData2 mPokerAnimationCData,
            ref LocalTransform mLocalTransform,
            ref Entity mEntity)
        {
            if (!mPokerAnimationCData.open)
            {
                return;
            }

            if (mPokerAnimationCData.trigger)
            {
                var startPt = mPokerAnimationCData.nowPt;
                var maxHeight = mPokerAnimationCData.maxHeight;
                var toRight = mPokerAnimationCData.btoRight;
                var vx_a = mPokerAnimationCData.vx_a;
                var vy_a = mPokerAnimationCData.vy_a;
                var nowPt = new Vector3(0, 0, 0);

                mPokerAnimationCData.vx += vx_a * dt;
                mPokerAnimationCData.vy += vy_a * dt;
                var vx = mPokerAnimationCData.vx;
                var vy = mPokerAnimationCData.vy;

                nowPt.x = startPt.x + vx * dt + 0.5f * vx_a * dt * dt;
                nowPt.y = startPt.y + vy * dt + 0.5f * vy_a * dt * dt;
                nowPt.z = startPt.z;

                if (nowPt.y < mPokerAnimationCData.minHeight)
                {
                    nowPt.y = mPokerAnimationCData.minHeight;
                    mPokerAnimationCData.vy *= -0.95f;
                }

                if (nowPt.y > mPokerAnimationCData.maxHeight)
                {
                    nowPt.y = mPokerAnimationCData.maxHeight;
                    mPokerAnimationCData.vy = 0;
                    mPokerAnimationCData.maxHeight = mPokerAnimationCData.maxHeight * 0.7f;
                }

                mPokerAnimationCData.nowPt = nowPt;
                mLocalTransform.Position = nowPt;

                CloneFlyObj(entityIndexInQuery, ref mPokerAnimationCData, ref mLocalTransform);

                if (Mathf.Abs(nowPt.x) > mPokerAnimationCData.maxWidth + 100)
                {
                    mPokerAnimationCData.open = false;
                }
            }
            else
            {
                mPokerAnimationCData.triggerDelay -= dt;
                if (mPokerAnimationCData.triggerDelay <= 0)
                {
                    mPokerAnimationCData.trigger = true;
                    mLocalTransform.Scale = 0;
                }
            }
        }

        private void CloneFlyObj(
            int entityIndexInQuery,
            ref PokerAnimationCData2 otherPokerAnimationCData,
            ref LocalTransform otherLocalTransform)
        {
            Unity.Assertions.Assert.IsTrue(Prefab != Entity.Null, "Prefab == Entity.Null");
            Unity.Assertions.Assert.IsTrue(Parent != Entity.Null, "Parent == Entity.Null");

            int colorType = otherPokerAnimationCData.color;
            int cardNum = otherPokerAnimationCData.value;

            Entity mTargetEntity = ECB.Instantiate(entityIndexInQuery, Prefab);

            var mParent = new Parent();
            mParent.Value = Parent;
            ECB.AddComponent(entityIndexInQuery, mTargetEntity, mParent);

            var mPokerItemCData = new PokerItemCData();
            mPokerItemCData.color = colorType;
            mPokerItemCData.cardNum = cardNum;
            mPokerItemCData.nCardId = colorType * 13 + cardNum;
            ECB.AddComponent(entityIndexInQuery, mTargetEntity, mPokerItemCData);

            var mLocalTransform = new LocalTransform();
            mLocalTransform.Position = otherLocalTransform.Position;
            mLocalTransform.Rotation = quaternion.identity;
            mLocalTransform.Scale = OriScale.x;
            ECB.AddComponent(entityIndexInQuery, mTargetEntity, mLocalTransform);

            ECB.AddComponent(entityIndexInQuery, mTargetEntity, new SetPokerItemDataEvent());
            ECB.AddComponent(entityIndexInQuery, mTargetEntity, new PokerTimerRemoveCData() { mRomveCdTime = 6.0f });
            ECB.AddComponent(entityIndexInQuery, mTargetEntity, new SetPokerItemSortingOrderEvent());
        }

    }

    public void SetGameObjectSprite(Entity mEntity_PokerItem, PokerItemCData mData)
    {
        SpriteAtlas atl_game = PokerGoMgr.Instance.mPokerAtlas;
        string p_name = "di_" + mData.cardNum + "_" + mData.color;
        SpriteAtlas atl_game1 = PokerGoMgr.Instance.mPokerBackAtlas;
        string p_name_back = "cardback_1";

        var mEntity_Poker = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Sprite");
        var mEntity_Back = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Back");
        var mSpriteRenderer1 = SystemAPI.ManagedAPI.GetComponent<SpriteRenderer>(mEntity_Poker);
        var mSpriteRenderer2 = SystemAPI.ManagedAPI.GetComponent<SpriteRenderer>(mEntity_Back);

        SpriteAtlas mSpriteAtlas = PokerGoMgr.Instance.mPokerAtlas;
        Sprite spri_bg = mSpriteAtlas.GetSprite(p_name);
        mSpriteRenderer1.sprite = spri_bg;
        mSpriteRenderer1.sortingOrder = mData.cardNum;

        mSpriteAtlas = PokerGoMgr.Instance.mPokerAtlas;
        spri_bg = mSpriteAtlas.GetSprite(p_name_back);
        mSpriteRenderer2.sprite = spri_bg;
        mSpriteRenderer2.sortingOrder = 0;

        Unity.Assertions.Assert.IsTrue(mSpriteRenderer1.sortingOrder < 14, "mSpriteRenderer1.sortingOrder: " + mSpriteRenderer1.sortingOrder);
        this.onSetNormal(mEntity_PokerItem);
    }

    int nOrderId = 0;
    public void UpdatePokerSortingOrderInFly(Entity mEntity_PokerItem)
    {
        var mData = SystemAPI.GetComponentRW<PokerItemCData>(mEntity_PokerItem);
        var mEntity_Poker = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Sprite");
        var mEntity_Back = ECSHelper.FindChildEntityByName(EntityManager, mEntity_PokerItem, "Back");
        var mSpriteRenderer1 = SystemAPI.ManagedAPI.GetComponent<SpriteRenderer>(mEntity_Poker);
        var mSpriteRenderer2 = SystemAPI.ManagedAPI.GetComponent<SpriteRenderer>(mEntity_Back);
        mSpriteRenderer1.sortingOrder = nOrderId++ + 100;
        mSpriteRenderer2.sortingOrder = 0;

        Unity.Assertions.Assert.IsTrue(mSpriteRenderer1.sortingOrder >= 100, "mSpriteRenderer1.sortingOrder: " + mSpriteRenderer1.sortingOrder);
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
    }

    // 检查是否最后一个队列中的最后一个，标志动画结束。   
    Entity addStaticCard(float3 pt, int colorType, int value)
    {
        RefRW<PokerSystemSingleton> mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();

        Unity.Assertions.Assert.IsTrue(mInstance.ValueRO.Prefab != Entity.Null, "mInstance.Prefab == Entity.Null");

        Entity mTargetEntity = EntityManager.Instantiate(mInstance.ValueRO.Prefab);
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

        mPokerItemCData.ValueRW.cardNum = value;
        mPokerItemCData.ValueRW.color = colorType;
        mPokerItemCData.ValueRW.nCardId = colorType * 13 + value;

        SetGameObjectSprite(mTargetEntity, mPokerItemCData.ValueRO);
        return mTargetEntity;
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
