using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(InitializationSystemGroup))] // 在初始化阶段运行
public partial class PokerSystemInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        // 等待某个 Entity（比如配置管理器）带有 InitializationCompleteTag
        if (!SystemAPI.TryGetSingletonEntity<PokerSystemInitFinishCData>(out Entity mEntity))
        {
            return;
        }

        InitSingleton();
        Enabled = false;
        EntityManager.DestroyEntity(mEntity);
    }

    private void InitSingleton()
    {
        PokerSystemCData mTargetData = default;
        foreach (var mData in SystemAPI.Query<RefRO<PokerSystemCData>>())
        {
            mTargetData = mData.ValueRO;
        }

        //检查是否已存在单例实体
        if (!SystemAPI.TryGetSingletonEntity<PokerSystemSingleton>(out Entity entity))
        {
            // 1. 创建实体
            entity = EntityManager.CreateEntity();
            // 2. 添加组件（此时组件是默认值）
            EntityManager.AddComponent<PokerSystemSingleton>(entity);

            // 不存在，创建单例实体并添加组件
            var mData = new PokerSystemSingleton();
            mData.worldPos_start = PokerGoMgr.Instance.startPt_obj.transform.position;
            mData.worldPos_ScreenTopLeft = PokerGoMgr.Instance.TopLeft_obj.transform.position;
            mData.worldPos_ScreenBottomRight = PokerGoMgr.Instance.BottomRight_obj.transform.position;
            mData.State = PokerGameState.Start;
            SystemAPI.SetSingleton(mData);
        }

        //这里就是把一些关键节点 找到对应的实体
        foreach (var (mData, mEntity) in SystemAPI.Query<RefRO<NodeTagCData>>().WithEntityAccess())
        {
            if (mData.ValueRO.Value == "cardsnode")
            {
                var mSingle = SystemAPI.GetSingleton<PokerSystemSingleton>();
                mSingle.cardsNode = mEntity;
            }
        }

    }
}
