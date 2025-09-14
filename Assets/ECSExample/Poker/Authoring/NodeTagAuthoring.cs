using Unity.Entities;
using UnityEngine;

//�˿��� Image ������
public class NodeTagAuthoring : MonoBehaviour
{
    class mBaker : Baker<NodeTagAuthoring>
    {
        public override void Bake(NodeTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NodeTagCData() {Value = authoring.gameObject.name});
        }
    }
}

