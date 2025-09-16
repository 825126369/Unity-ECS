using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(InitializationSystemGroup))] // �ڳ�ʼ���׶�����
public partial class PokerSystemInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        // �ȴ�ĳ�� Entity���������ù����������� InitializationCompleteTag
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
        //PokerSystemCData mTargetData = default;
        //foreach (var mData in SystemAPI.Query<RefRO<PokerSystemCData>>())
        //{
        //    mTargetData = mData.ValueRO;
        //}

        //����Ƿ��Ѵ��ڵ���ʵ��
        if (!SystemAPI.TryGetSingletonEntity<PokerSystemSingleton>(out Entity entity))
        {
            entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<PokerSystemSingleton>(entity);
        }

        var mPokerSystemSingleton = SystemAPI.GetSingleton<PokerSystemSingleton>();
        mPokerSystemSingleton.worldPos_start = PokerGoMgr.Instance.startPt_obj.transform.position;
        mPokerSystemSingleton.worldPos_ScreenTopLeft = PokerGoMgr.Instance.TopLeft_obj.transform.position;
        mPokerSystemSingleton.worldPos_ScreenBottomRight = PokerGoMgr.Instance.BottomRight_obj.transform.position;
        mPokerSystemSingleton.State = PokerGameState.Start;

        //������ǰ�һЩ�ؼ��ڵ� �ҵ���Ӧ��ʵ��
        foreach (var (mData, mEntity) in SystemAPI.Query<RefRO<NodeTagCData>>().WithEntityAccess())
        {
            if (mData.ValueRO.Value == "cardsnode")
            {
                mPokerSystemSingleton.cardsNode = mEntity;
            }
        }

        foreach (var (mData, mEntity) in SystemAPI.Query<RefRO<PokerPoolCData>>().WithEntityAccess())
        {
            mPokerSystemSingleton.Prefab = mData.ValueRO.Prefab;
        }

        Unity.Assertions.Assert.IsTrue(mPokerSystemSingleton.Prefab != Entity.Null, "Prefab == null");
        Unity.Assertions.Assert.IsTrue(mPokerSystemSingleton.cardsNode != Entity.Null, "cardsNode == null");
        SystemAPI.SetSingleton(mPokerSystemSingleton);
    }
}
