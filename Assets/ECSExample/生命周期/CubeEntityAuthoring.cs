using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CubeEntityAuthoring : MonoBehaviour
{
    //烘培得加到ECS 场景中，只有 SubScene 指定的 Scene中的物体才会烘培
    class MyBaker : Baker<CubeEntityAuthoring>
    {
        public override void Bake(CubeEntityAuthoring authoring)
        {
            Debug.Log("烘培: CubeToEntityAuthoring");
            var entity = GetEntity(TransformUsageFlags.Renderable);
            //这里必须加个 AddComponent ，否则场景中不会显示这个物体
            //AddComponent(entity, new LocalTransform());
            //AddComponent(entity, new BoidObstacle());

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

    public struct BoidObstacle : IComponentData
    {
    }
}
