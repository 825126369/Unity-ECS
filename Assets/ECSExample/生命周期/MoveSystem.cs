using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class CubeToEntityAuthoring : MonoBehaviour
{
    //烘培得加到ECS 场景中，只有 SubScene 指定的 Scene中的物体才会烘培
    class MyBaker : Baker<CubeToEntityAuthoring>
    {
        public override void Bake(CubeToEntityAuthoring authoring)
        {
            Debug.Log("烘培: CubeToEntityAuthoring");
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            //// ✅ 添加 Transform
            //AddComponent(entity, new LocalTransform
            //{
            //    Position = authoring.transform.position,
            //    Rotation = quaternion.identity,
            //    Scale = 1f
            //});

            //// ✅ 添加 RenderMesh（关键！）
            //AddComponent(entity, new RenderMesh
            //{
            //    mesh = authoring.meshFilter.sharedMesh,
            //    material = authoring.meshRenderer.sharedMaterial,
            //    // castShadow = ShadowCastingMode.On,
            //    // receiveShadows = true,
            //});
        }
    }
}

public partial class MoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        //Entities.ForEach((ref LocalTransform transform) =>
        //{
        //    Debug.Log("移动");
        //    transform.Position += new float3(0.5f, 0, 0) * deltaTime; // 每秒向右移动 1 单位

        //    // 或者直接设置一个新位置
        //    transform.Position = new float3(0, 0, 0);
        //    // 也可以同时修改旋转和缩放
        //    // transform.Rotation = quaternion.RotateY(transform.Rotation.value, 0.1f * deltaTime);
        //    // transform.Scale = 1.5f;

        //}).ScheduleParallel(); // 使用 Job System 并行执行

        // 查询组件，更新位置
        //foreach (var transform in
        //         SystemAPI.Query<RefRW<LocalTransform>>())
        //{
        //    Debug.Log("移动");
        //    transform.ValueRW.Position += new float3(0.5f, 0, 0) * deltaTime;
        //}
    }
}
