using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
[BurstCompile]
public partial class PokerAniSystem : SystemBase
{
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
        RefRW<PokerSystemSingleton> mInstance = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        if (mInstance.ValueRO.State == PokerGameState.None)
        {
            foreach (var (mEvent, mEntity) in SystemAPI.Query<RefRO<StartDoAniEvent>>().WithEntityAccess())
            {
                mInstance.ValueRW.State = PokerGameState.Start;
                break;
            }
        }
        else if (mInstance.ValueRO.State == PokerGameState.Start)
        {
            Debug.Log($"PokerAniSystem OnUpdate - Frame: {UnityEngine.Time.frameCount}");
            NativeArray<int> colors = new NativeArray<int>(4, Allocator.Persistent);
            colors[0] = 0;
            colors[1] = 1;
            colors[2] = 2;
            colors[3] = 3;
            Show(mInstance, colors, mInstance.ValueRO.worldPos_start, 0, null);
            mInstance.ValueRW.State = PokerGameState.Playing;
        }
        else if (mInstance.ValueRO.State == PokerGameState.Playing)
        {
            foreach (var v in mInstance.ValueRW.animationEntitys)
            {
                this.updateAnimation(mInstance, v, deltaTime);
            }
        }
        else if (mInstance.ValueRO.State == PokerGameState.End)
        {
            // UnityMainThreadDispatcher.Instance.Enqueue(new MainThreadData_End());
        }
    }

    public void Show(RefRW<PokerSystemSingleton> mInstance, NativeArray<int> colors, float3 startPt_w, int offsetX, Action callback)
    {
        mInstance.ValueRW.animationOver = false;
        mInstance.ValueRW.colNodes_Dic = new NativeHashMap<int, NativeList<Entity>>(54, Allocator.Persistent);
        mInstance.ValueRW.allNodes = new NativeList<Entity>(54, Allocator.Persistent);
        mInstance.ValueRW.animationEntitys = new NativeList<Entity>(54, Allocator.Persistent);
        mInstance.ValueRW.colors = colors;
        //mInstance.callBack = callback;

        float3 firstPt_l = mInstance.ValueRO.worldPos_start;
        float delay = 0.1f;
        for (int i = 0; i < colors.Length; i++)
        {
            int color = colors[i];
            int offset = offsetX * i;
            Vector3 frompt = new Vector3(firstPt_l.x + i * (PokerSystemSingleton.CardWidth - 1) + offset, firstPt_l.y, firstPt_l.z);
            delay = i * 0.5f;
            float delayvalue = 0;
            float delayoffset = 0.1f;
            for (int j = 13; j > 0; j--)
            {
                this.showAnimation_Default_ColValue(mInstance, i, frompt, delay + delayvalue, color, j, offsetX);
                delayvalue += delayoffset;
            }
        }
    }

    void showAnimation_Default_ColValue(RefRW<PokerSystemSingleton> mInstance, int colindex, Vector3 pt, float delay, int color, int value, int offsetX = 0)
    {
        Entity mEntity = this.addStaticCard(mInstance, pt, color, value, colindex);

        PokerAnimationCData mPokerAnimationCData = EntityManager.GetComponentData<PokerAnimationCData>(mEntity);
        mPokerAnimationCData.Init(colindex, color, value);
        mPokerAnimationCData.open = true;
        mPokerAnimationCData.trigger = false;
        mPokerAnimationCData.triggerDelay = delay;
        mPokerAnimationCData.btoRight = PokerAnimationCData.toRight(colindex);
        mPokerAnimationCData.startPt = pt;
        mPokerAnimationCData.nowPt = pt;
        mPokerAnimationCData.mEntity = mEntity;
        mPokerAnimationCData.maxHeight = mInstance.ValueRO.maxHeight;
        mPokerAnimationCData.vx = PokerAnimationCData.randomVx();
        mPokerAnimationCData.vx_a = 0;
        mPokerAnimationCData.vy = PokerAnimationCData.randomVy();

        if (!mPokerAnimationCData.btoRight)
        {
            mPokerAnimationCData.vx *= -1;
        }

        mPokerAnimationCData.vy_a = PokerAnimationCData.randomVy_a();
        mPokerAnimationCData.deltTime = 0;
        mInstance.ValueRO.animationEntitys.Add(mEntity);
        mInstance.ValueRO.allNodes.Add(mEntity);
    }

    // 检查是否最后一个队列中的最后一个，标志动画结束。   
    Entity addStaticCard(RefRW<PokerSystemSingleton> mInstance, float3 pt, int colorType, int value, int colindex)
    {
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
                    this.DoDestroyAction(mInstance);
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
        var mPokerItemCData = EntityManager.GetComponentData<PokerItemCData>(mTargetEntity);
        var mParent = SystemAPI.GetComponentRW<Parent>(mTargetEntity);

       // mParent.ValueRW.Value = mInstance.ValueRW.cardsNode;
        mLocalTransform.ValueRW.Position = pt;
        mPokerItemCData.initByNum(value, colorType);

        nodeArrs.Add(mTargetEntity);
        return mTargetEntity;
    }

    void updateAnimation(RefRW<PokerSystemSingleton> mInstance, Entity mEntity, float dt)
    {
        PokerAnimationCData mPokerAnimationCData = EntityManager.GetComponentData<PokerAnimationCData>(mEntity);
        LocalTransform mLocalTransform = EntityManager.GetComponentData<LocalTransform>(mEntity);
        if (!mPokerAnimationCData.open)
        {
            return;
        }

        if (mPokerAnimationCData.trigger)
        {
            // 没有节点的时候，不更新。
            if (mEntity == Entity.Null)
            {
                return;
            }

            var deltTime = mPokerAnimationCData.deltTime;
            var startPt = mPokerAnimationCData.nowPt;
            var maxHeight = mPokerAnimationCData.maxHeight;
            var toRight = mPokerAnimationCData.btoRight;
            var vx_a = mPokerAnimationCData.vx_a;
            var vy_a = mPokerAnimationCData.vy_a;
            var nowPt = new Vector3(0, 0, 0);
            // 匀变速直线运动位移公式：a=dv/dt，
            // 距离 x = v0t+1/2·at^2

            // 现在速度
            mPokerAnimationCData.vx += vx_a * dt;
            mPokerAnimationCData.vy += vy_a * dt;
            var vx = mPokerAnimationCData.vx;
            var vy = mPokerAnimationCData.vy;

            nowPt.x = (float)(startPt.x + vx * dt + 0.5f * vx_a * dt * dt);
            nowPt.y = (float)(startPt.y + vy * dt + 0.5f * vy_a * dt * dt);
            nowPt.z = startPt.z;

            // 垂直. 小于最低值。
            if (nowPt.y < mInstance.ValueRO.minHeight)
            {
                nowPt.y = mInstance.ValueRO.minHeight;
                mPokerAnimationCData.vy *= -0.95f;  //转变方向
            }

            if (nowPt.y > mPokerAnimationCData.maxHeight)
            {
                nowPt.y = mPokerAnimationCData.maxHeight;
                mPokerAnimationCData.vy = 0;
                mPokerAnimationCData.maxHeight = mPokerAnimationCData.maxHeight * 0.8f;
            }

            bool willRemove = false;
            if (nowPt.x < mInstance.ValueRO.minWidth - PokerSystemSingleton.CardWidth)
            {
                nowPt.x = mInstance.ValueRO.minWidth;
                willRemove = true;
            }

            if (nowPt.x > mInstance.ValueRO.maxWidth + PokerSystemSingleton.CardWidth)
            {
                nowPt.x = mInstance.ValueRO.maxWidth;
                mPokerAnimationCData.vx *= -1;
                willRemove = true;
            }

            // 每两帧之间 添加
            mPokerAnimationCData.checktimes += 1;
            mLocalTransform.Position = nowPt;
            mPokerAnimationCData.nowPt = nowPt;

            // 碰到边界，不在产生新的额
            if (mPokerAnimationCData.open)
            {
                mLocalTransform.Position = nowPt;
                if (willRemove)
                {
                    mPokerAnimationCData.open = false;
                    if (mPokerAnimationCData.value == 6 && mPokerAnimationCData.index == 2)
                    {
                        this.onAnimatinCallBack();
                        this.DoDestroyAction(mInstance);
                    }
                    return;
                }
            }
        }
        else
        {
            mPokerAnimationCData.triggerDelay -= dt;
            if (mPokerAnimationCData.triggerDelay <= 0)
            {
                mPokerAnimationCData.trigger = true;
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

    public void DoDestroyAction(RefRW<PokerSystemSingleton> mInstance)
    {
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
        mInstance.ValueRO.animationEntitys.Clear();

        foreach (var v in mInstance.ValueRO.allNodes)
        {
            EntityPoolManager.Instance.Recycle(v);
        }
        mInstance.ValueRO.allNodes.Clear();
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
