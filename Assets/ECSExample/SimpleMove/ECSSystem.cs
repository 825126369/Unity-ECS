using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

//性能优化：避免无意义的空更新。比如一个只处理“子弹”的系统，当场景中没有子弹时，根本不需要运行。
//逻辑安全：确保系统运行时，依赖的数据一定存在，避免空查询或无效操作。
//代码清晰：明确表达“这个系统只在有相关实体时才工作”。
[RequireMatchingQueriesForUpdate]
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

