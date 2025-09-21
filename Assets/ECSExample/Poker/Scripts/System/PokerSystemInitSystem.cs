using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
public partial class PokerSystemInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        Debug.Log($"PokerSystemInitSystem OnUpdate - Frame: {UnityEngine.Time.frameCount}");
        //PokerGoMgr 初始化完成后，触发事件, 这个事件
        if (!SystemAPI.TryGetSingletonEntity<PokerGoMgrInitFinishEvent>(out Entity mTempEntity))
        {
            return;
        }

        if (InitSingleton())
        {
            Enabled = false;
            EntityManager.DestroyEntity(mTempEntity);
        }
    }

    private bool InitSingleton()
    {
        //检查是否已存在单例实体
        if (!SystemAPI.HasSingleton<PokerSystemSingleton>())
        {
            EntityManager.CreateEntity(typeof(PokerSystemSingleton));
        }

        var mPokerSystemSingleton = SystemAPI.GetSingletonRW<PokerSystemSingleton>();
        mPokerSystemSingleton.ValueRW.worldPos_start_list = new NativeArray<float3>(4, Allocator.Persistent);
        mPokerSystemSingleton.ValueRW.worldPos_start_list[0] = PokerGoMgr.Instance.startPt_obj1.transform.position;
        mPokerSystemSingleton.ValueRW.worldPos_start_list[1] = PokerGoMgr.Instance.startPt_obj2.transform.position;
        mPokerSystemSingleton.ValueRW.worldPos_start_list[2] = PokerGoMgr.Instance.startPt_obj3.transform.position;
        mPokerSystemSingleton.ValueRW.worldPos_start_list[3] = PokerGoMgr.Instance.startPt_obj4.transform.position;

        mPokerSystemSingleton.ValueRW.worldScale_start_list = new NativeArray<float3>(4, Allocator.Persistent);
        mPokerSystemSingleton.ValueRW.worldScale_start_list[0] = GoHelper.GetUIToWorldScale(PokerGoMgr.Instance.startPt_obj1);
        mPokerSystemSingleton.ValueRW.worldScale_start_list[1] = GoHelper.GetUIToWorldScale(PokerGoMgr.Instance.startPt_obj1);
        mPokerSystemSingleton.ValueRW.worldScale_start_list[2] = GoHelper.GetUIToWorldScale(PokerGoMgr.Instance.startPt_obj1);
        mPokerSystemSingleton.ValueRW.worldScale_start_list[3] = GoHelper.GetUIToWorldScale(PokerGoMgr.Instance.startPt_obj1);

        mPokerSystemSingleton.ValueRW.worldPos_ScreenTopLeft = PokerGoMgr.Instance.TopLeft_obj.transform.position;
        mPokerSystemSingleton.ValueRW.worldPos_ScreenBottomRight = PokerGoMgr.Instance.BottomRight_obj.transform.position;
        mPokerSystemSingleton.ValueRW.maxHeight = PokerGoMgr.Instance.TopLeft_obj.transform.position.y + 200;
        mPokerSystemSingleton.ValueRW.minHeight = PokerGoMgr.Instance.BottomRight_obj.transform.position.y;
        mPokerSystemSingleton.ValueRW.maxWidth = PokerGoMgr.Instance.BottomRight_obj.transform.position.x + 100;
        mPokerSystemSingleton.ValueRW.minWidth = PokerGoMgr.Instance.TopLeft_obj.transform.position.x - 100;

        mPokerSystemSingleton.ValueRW.State = PokerGameState.None;
        mPokerSystemSingleton.ValueRW.nAniType =  PokerAniType.FlyFullScreen3;

        //这里就是把一些关键节点 找到对应的实体，这些都是烘培的实体，到底是第几帧 加载了，不知道
        foreach (var (mData, mEntity) in SystemAPI.Query<RefRO<GameObjectCData>>().WithEntityAccess())
        {
            if (mData.ValueRO.Name == "cardsnode")
            {
                mPokerSystemSingleton.ValueRW.cardsNode = mEntity;
            }
        }
        
        foreach (var (mData, mEntity) in SystemAPI.Query<RefRO<PokerPoolCData>>().WithEntityAccess())
        {
            mPokerSystemSingleton.ValueRW.Prefab = mData.ValueRO.Prefab;
        }

        //检查是否 查询就绪了, 有可能这个是第一帧，第三帧，到底第几帧 查询状态就绪不知道
        //这些都是烘培的实体，到底是第几帧 加载了，不知道
        if (mPokerSystemSingleton.ValueRW.Prefab == Entity.Null || 
            mPokerSystemSingleton.ValueRW.cardsNode == Entity.Null)
        {
            return false;
        }
        return true;
    }
}
