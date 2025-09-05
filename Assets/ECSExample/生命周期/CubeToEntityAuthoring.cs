using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CubeToEntityAuthoring : MonoBehaviour
{
    class CubeToEntityBaker : Baker<CubeToEntityAuthoring>
    {
        public override void Bake(CubeToEntityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new LocalTransform());
            //AddComponent(entity, new LocalTransform
            //{
            //    Value = authoring.Speed
            //});
        }
    }
}
