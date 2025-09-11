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
        //如果系统中途被 Enabled = false，再设为 true，它会再次调用； 类似 MonoBehaviour.Enable
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();
    }
    
    protected override void OnCreateForCompiler()
    {
        base.OnCreateForCompiler();
        //开发者不需要管这个
    }

    protected override void OnUpdate()
    {
        Enabled = true;
    }
}

