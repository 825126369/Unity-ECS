using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
//[UpdateInGroup(typeof(SimulationSystemGroup))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateInGroup(typeof(PresentationSystemGroup))]
//[UpdateInGroup(typeof(LateSimulationSystemGroup))]
//[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct ECSSystem : ISystem
{
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //UnityEngine.Debug.Log("ECSSystem: OnCreate");
    }

    //[BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        //UnityEngine.Debug.Log("ECSSystem: OnDestroy");
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
       //UnityEngine.Debug.Log("ECSSystem: OnUpdate");
    }
}

public partial class ECSSystem2 : SystemBase
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
        //���ϵͳ��;�� Enabled = false������Ϊ true�������ٴε��ã� ���� MonoBehaviour.Enable
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();
    }
    
    protected override void OnCreateForCompiler()
    {
        base.OnCreateForCompiler();
        //�����߲���Ҫ�����
    }

    protected override void OnUpdate()
    {
        Enabled = true;
    }
}

