using Unity.Burst;
using Unity.Entities;

public partial struct ECSSystem : ISystem
{
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        UnityEngine.Debug.Log("ECSSystem: OnCreate");
    }

    //[BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        UnityEngine.Debug.Log("ECSSystem: OnDestroy");
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
       // UnityEngine.Debug.Log("ECSSystem: OnUpdate");
    }
}
