using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))] // �ڳ�ʼ���׶�����
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
