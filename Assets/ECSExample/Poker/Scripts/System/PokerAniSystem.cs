//using System;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//[RequireMatchingQueriesForUpdate]
//[BurstCompile]
//public partial class PokerAniSystem : SystemBase
//{
//    protected override void OnCreate()
//    {
//        base.OnCreate();
//    }

//    protected override void OnDestroy()
//    {
//        base.OnDestroy();
//    }

//    protected override void OnStartRunning()
//    {
//        base.OnStartRunning();
//        //如果系统中途被 Enabled = false，再设为 true，它会再次调用； 类似 MonoBehaviour.Enable
//    }

//    protected override void OnStopRunning()
//    {
//        base.OnStopRunning();
//    }

//    protected override void OnUpdate()
//    {
//        float deltaTime = SystemAPI.Time.DeltaTime;
//        PokerSystemSingleton mGlobalData = SystemAPI.GetSingleton<PokerSystemSingleton>();
//        if (mGlobalData.State == PokerGameState.Start)
//        {
//            NativeArray<int> colors = new NativeArray<int>(4, Allocator.Persistent);
//            colors[0] = 0;
//            colors[1] = 1;
//            colors[2] = 2;
//            colors[3] = 3;
//            Show(colors, mGlobalData.worldPos_start, 0, null);
//            mGlobalData.State = PokerGameState.Playing;
//        }
//        else if (mGlobalData.State == PokerGameState.Playing)
//        {
//            for (int index = 0; index < mGlobalData.animationEntitys.Length; index++)
//            {
//                var entity = mGlobalData.animationEntitys[index];
//                this.updateAnimation(entity, deltaTime);
//            }
//        }
//        else if (mGlobalData.State == PokerGameState.End)
//        {
//            // UnityMainThreadDispatcher.Instance.Enqueue(new MainThreadData_End());
//        }
//        else
//        {
//            Unity.Assertions.Assert.IsTrue(false);
//        }
//    }

//    public void Show(NativeArray<int> colors, float3 startPt_w, int offsetX, Action callback)
//    {
//        PokerSystemSingleton mInstance = SystemAPI.GetSingleton<PokerSystemSingleton>();

//        mInstance.animationOver = false;
//        mInstance.colNodes_Dic = new NativeHashMap<int, NativeList<Entity>>();
//        mInstance.allNodes = new NativeList<Entity>();
//        mInstance.animationEntitys = new NativeList<Entity>();
//        mInstance.colors = colors;
//        //mInstance.callBack = callback;

//        float3 firstPt_l = mInstance.worldPos_start;
//        float delay = 0.1f;
//        for (int i = 0; i < colors.Length; i++)
//        {
//            int color = colors[i];
//            int offset = offsetX * i;
//            Vector3 frompt = new Vector3(firstPt_l.x + i * (PokerSystemSingleton.CardWidth - 1) + offset, firstPt_l.y, firstPt_l.z);
//            delay = i * 0.5f;
//            float delayvalue = 0;
//            float delayoffset = 0.1f;
//            for (int j = 13; j > 0; j--)
//            {
//                this.showAnimation_Default_ColValue(i, frompt, delay + delayvalue, color, j, offsetX);
//                delayvalue += delayoffset;
//            }
//        }

//    }

//    void showAnimation_Default_ColValue(int colindex, Vector3 pt, float delay, int color, int value, int offsetX = 0)
//    {
//        PokerSystemSingleton mInstance = SystemAPI.GetSingleton<PokerSystemSingleton>();

//        (Entity node, bool) ret = this.addStaticCard(pt, color, value, colindex);
//        var startNode = ret.node;

//        var entity = EntityPoolManager.Instance.Spawn(mInstance.Prefab, PoolTagConst.Poker);

//        PokerAnimationCData mPokerAnimationCData = EntityManager.GetComponentData<PokerAnimationCData>(entity);
//        mPokerAnimationCData.Init(colindex, color, value);
//        mPokerAnimationCData.open = true;
//        mPokerAnimationCData.trigger = false;
//        mPokerAnimationCData.triggerDelay = delay;
//        mPokerAnimationCData.btoRight = PokerAnimationCData.toRight(colindex);
//        mPokerAnimationCData.startPt = pt;
//        mPokerAnimationCData.nowPt = pt;
//        mPokerAnimationCData.mEntity = entity;
//        mPokerAnimationCData.maxHeight = mInstance.maxHeight;
//        mPokerAnimationCData.vx = PokerAnimationCData.randomVx();
//        mPokerAnimationCData.vx_a = 0;
//        mPokerAnimationCData.vy = PokerAnimationCData.randomVy();

//        if (!mPokerAnimationCData.btoRight)
//        {
//            mPokerAnimationCData.vx *= -1;
//        }

//        mPokerAnimationCData.vy_a = PokerAnimationCData.randomVy_a();
//        mPokerAnimationCData.deltTime = 0;
//        mInstance.animationEntitys.Add(entity);
//    }

//    // 检查是否最后一个队列中的最后一个，标志动画结束。   
//    (Entity, bool) addStaticCard(float3 pt, int colorType, int value, int colindex)
//    {
//        PokerSystemSingleton mInstance = SystemAPI.GetSingleton<PokerSystemSingleton>();

//        int nodekey = PokerAnimationCData.getCardId(colorType, value);
//        if (!mInstance.colNodes_Dic.TryGetValue(nodekey, out NativeList<Entity> nodeArrs))
//        {
//            nodeArrs = new NativeList<Entity>(Allocator.Persistent);
//            mInstance.colNodes_Dic.Add(nodekey, nodeArrs);
//        }

//        bool firstNodeByNewValue = false;
//        if (value == 13 && nodeArrs.Length == 0)
//        {
//            firstNodeByNewValue = true;
//        }

//        Entity mTargetEntity = Entity.Null;
//        // 从后往前找，找到第一个。
//        for (int index = mInstance.animationEntitys.Length - 1; index >= 0; index--)
//        {
//            var mEntity = mInstance.animationEntitys[index];
//            var mPokerAnimationCData2 = EntityManager.GetComponentData<PokerAnimationCData>(mEntity);
//            if (mPokerAnimationCData2.color == colorType && mPokerAnimationCData2.value == value && mPokerAnimationCData2.index == colindex)
//            {
//                mTargetEntity = mEntity;
//                break;
//            }
//        }

//        if (nodeArrs.Length >= PokerSystemSingleton.CardsColTotal)
//        {
//            if (mTargetEntity == null)
//            {

//            }
//            else
//            {
//                var mPokerAnimationCData2 = EntityManager.GetComponentData<PokerAnimationCData>(mTargetEntity);
//                if (mPokerAnimationCData2.color == mInstance.colors[mInstance.colors.Length - 1] && mPokerAnimationCData2.value == 1)
//                {
//                    this.onAnimatinCallBack();
//                    this.DoDestroyAction();
//                }
//            }
//            return (Entity.Null, firstNodeByNewValue);
//        }

//        Entity startNode = EntityPoolManager.Instance.Spawn(mInstance.Prefab, PoolTagConst.Poker);
//        LocalTransform mLocalTransform = EntityManager.GetComponentData<LocalTransform>(startNode);
//        PokerAnimationCData mPokerAnimationCData = EntityManager.GetComponentData<PokerAnimationCData>(mTargetEntity);
//        PokerItemCData mPokerItemCData = EntityManager.GetComponentData<PokerItemCData>(mTargetEntity);

//        EntityManager.AddComponentData(startNode, new Parent { Value = mInstance.cardsNode });
//        mLocalTransform.Position = pt;
//        nodeArrs.Add(startNode);

//        mPokerItemCData.initByNum(value, colorType);
//        mInstance.allNodes.Add(startNode);
//        return (startNode, firstNodeByNewValue);
//    }

//    void updateAnimation(Entity mEntity, float dt)
//    {
//        PokerSystemSingleton mInstance = SystemAPI.GetSingleton<PokerSystemSingleton>();
//        PokerAnimationCData mPokerAnimationCData = EntityManager.GetComponentData<PokerAnimationCData>(mEntity);
//        LocalTransform mLocalTransform = EntityManager.GetComponentData<LocalTransform>(mEntity);
//        if (!mPokerAnimationCData.open)
//        {
//            return;
//        }

//        if (mPokerAnimationCData.trigger)
//        {
//            // 没有节点的时候，不更新。
//            if (mEntity == Entity.Null)
//            {
//                return;
//            }

//            var deltTime = mPokerAnimationCData.deltTime;
//            var startPt = mPokerAnimationCData.nowPt;
//            var maxHeight = mPokerAnimationCData.maxHeight;
//            var toRight = mPokerAnimationCData.btoRight;
//            var vx_a = mPokerAnimationCData.vx_a;
//            var vy_a = mPokerAnimationCData.vy_a;

//            var nowPt = new Vector3(0, 0, 0);
//            // 匀变速直线运动位移公式：a=dv/dt，
//            // 距离 x = v0t+1/2·at^2

//            // 现在速度
//            mPokerAnimationCData.vx += vx_a * dt;
//            mPokerAnimationCData.vy += vy_a * dt;
//            var vx = mPokerAnimationCData.vx;
//            var vy = mPokerAnimationCData.vy;

//            nowPt.x = (float)(startPt.x + vx * dt + 0.5f * vx_a * dt * dt);
//            nowPt.y = (float)(startPt.y + vy * dt + 0.5f * vy_a * dt * dt);
//            nowPt.z = startPt.z;

//            // 垂直. 小于最低值。
//            if (nowPt.y < mInstance.minHeight)
//            {
//                nowPt.y = mInstance.minHeight;
//                mPokerAnimationCData.vy *= -0.95f;  //转变方向
//            }

//            if (nowPt.y > mPokerAnimationCData.maxHeight)
//            {
//                nowPt.y = mPokerAnimationCData.maxHeight;
//                mPokerAnimationCData.vy = 0;
//                mPokerAnimationCData.maxHeight = mPokerAnimationCData.maxHeight * 0.8f;
//            }

//            bool willRemove = false;
//            if (nowPt.x < mInstance.minWidth - PokerSystemSingleton.CardWidth)
//            {
//                nowPt.x = mInstance.minWidth;
//                willRemove = true;
//            }

//            if (nowPt.x > mInstance.maxWidth + PokerSystemSingleton.CardWidth)
//            {
//                nowPt.x = mInstance.maxWidth;
//                mPokerAnimationCData.vx *= -1;
//                willRemove = true;
//            }

//            // 每两帧之间 添加
//            mPokerAnimationCData.checktimes += 1;
//            mLocalTransform.Position = nowPt;
//            mPokerAnimationCData.nowPt = nowPt;

//            // 碰到边界，不在产生新的额
//            if (mPokerAnimationCData.open)
//            {
//                mLocalTransform.Position = nowPt;
//                if (willRemove)
//                {
//                    mPokerAnimationCData.open = false;
//                    if (mPokerAnimationCData.value == 6 && mPokerAnimationCData.index == 2)
//                    {
//                        this.onAnimatinCallBack();
//                        this.DoDestroyAction();
//                    }
//                    return;
//                }
//            }
//        }
//        else
//        {
//            mPokerAnimationCData.triggerDelay -= dt;
//            if (mPokerAnimationCData.triggerDelay <= 0)
//            {
//                mPokerAnimationCData.trigger = true;
//            }
//        }
//    }

//    void onAnimatinCallBack()
//    {
//        PokerSystemSingleton mInstance = SystemAPI.GetSingleton<PokerSystemSingleton>();
//        mInstance.State = PokerGameState.End;

//        //if (this.callBack != null)
//        //{
//        //    this.callBack();
//        //    this.callBack = null;
//        //}
//    }

//    public void DoDestroyAction()
//    {
//        PokerSystemSingleton mInstance = SystemAPI.GetSingleton<PokerSystemSingleton>();
//        if (mInstance.animationOver)
//        {
//            return;
//        }

//        mInstance.animationOver = true;
//        for (int index = 0; index < mInstance.animationEntitys.Length; index++)
//        {
//            var mEntity = mInstance.animationEntitys[index];
//            PokerAnimationCData mPokerAnimationCData = EntityManager.GetComponentData<PokerAnimationCData>(mEntity);
//            mPokerAnimationCData.open = false;
//        }
//        mInstance.animationEntitys.Clear();

//        foreach (var v in mInstance.allNodes)
//        {
//            EntityPoolManager.Instance.Recycle(v);
//        }
//        mInstance.allNodes.Clear();
//    }

//    public void onClick_Skip()
//    {
//        //this.skipNode.SetActive(false);
//        //AudioController.Instance.playSound(Sounds.button, 1);
//        //this.onAnimatinCallBack();
//        //this.DoDestroyAction();
//        //TAController.Instance.trackAnimationSkip();
//    }
//}
