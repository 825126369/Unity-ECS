using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CubeToEntityAuthoring : MonoBehaviour
{
    class MyBaker : Baker<CubeToEntityAuthoring>
    {
        public override void Bake(CubeToEntityAuthoring authoring)
        {
            Debug.Log("∫Ê≈‡: CubeToEntityAuthoring");
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BoidObstacle());
            //AddComponent(entity, new LocalTransform
            //{
            //    Value = authoring.Speed
            //});
        }
    }


}

public struct BoidObstacle : IComponentData
{
}
