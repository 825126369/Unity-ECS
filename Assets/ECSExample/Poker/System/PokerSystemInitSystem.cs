using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))] // 在初始化阶段运行
public class PokerSystemInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}
