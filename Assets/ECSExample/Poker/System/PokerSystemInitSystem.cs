using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;

[UpdateInGroup(typeof(InitializationSystemGroup))] // 在初始化阶段运行
public partial class PokerSystemInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        InitSingleton();
        Enabled = false;
    }

    private void InitSingleton()
    {
        PokerSystemIComponentData mTargetData = default;
        foreach (var mData in SystemAPI.Query<RefRO<PokerSystemIComponentData>>())
        {
            mTargetData = mData.ValueRO;
        }

        // 检查是否已存在单例实体
        if (!SystemAPI.TryGetSingletonEntity<PokerSystemSingleton>(out Entity entity))
        {
            // 不存在，创建单例实体并添加组件
            var mData = new PokerSystemSingleton();
            mData.worldPos_start = mTargetData.worldPos_start;
            mData.worldPos_ScreenTopLeft = mTargetData.worldPos_ScreenTopLeft;
            mData.worldPos_ScreenBottomRight = mTargetData.worldPos_ScreenBottomRight;
            mData.State = PokerGameState.Start;
            SystemAPI.SetSingleton(mData);
        }
    }
}
