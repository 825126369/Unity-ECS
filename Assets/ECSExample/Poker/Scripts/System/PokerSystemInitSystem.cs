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
        PokerSystemCData mTargetData = default;
        foreach (var mData in SystemAPI.Query<RefRO<PokerSystemCData>>())
        {
            mTargetData = mData.ValueRO;
        }

        //����Ƿ��Ѵ��ڵ���ʵ��
        if (!SystemAPI.TryGetSingletonEntity<PokerSystemSingleton>(out Entity entity))
        {
            // 1. ����ʵ��
            entity = EntityManager.CreateEntity();
            // 2. ����������ʱ�����Ĭ��ֵ��
            EntityManager.AddComponent<PokerSystemSingleton>(entity);

            // �����ڣ���������ʵ�岢������
            var mData = new PokerSystemSingleton();
            mData.worldPos_start = PokerGoMgr.Instance.startPt_obj.transform.position;
            mData.worldPos_ScreenTopLeft = PokerGoMgr.Instance.TopLeft_obj.transform.position;
            mData.worldPos_ScreenBottomRight = PokerGoMgr.Instance.BottomRight_obj.transform.position;
            mData.State = PokerGameState.Start;
            SystemAPI.SetSingleton(mData);
        }

        //������ǰ�һЩ�ؼ��ڵ� �ҵ���Ӧ��ʵ��
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
