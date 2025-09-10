using Unity.Entities;
using UnityEngine;

public class CubeEntityAuthoring : MonoBehaviour
{
    //烘培得加到ECS 场景中，只有 SubScene 指定的 Scene中的物体才会烘培
    class MyBaker : Baker<CubeEntityAuthoring>
    {
        public override void Bake(CubeEntityAuthoring authoring)
        {
            Debug.Log("烘培: CubeToEntityAuthoring");
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveParam());
            SetComponent(entity, new MoveParam() { x = 5});
            AddComponent<SimpleMoveObj>(entity);

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

public struct SimpleMoveObj : IComponentData
{
    
}

public struct MoveParam : IComponentData
{
    public float x;
}
