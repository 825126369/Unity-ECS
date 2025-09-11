using Unity.Entities;
using UnityEngine;

//�˿��� Image ������
public class PokerPoolAuthoring : MonoBehaviour
{
    public GameObject goPrefab;
    public int nPoolCount = 100;

    class mBaker : Baker<PokerPoolAuthoring>
    {
        public override void Bake(PokerPoolAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PokerPoolObj
            {
                Prefab = GetEntity(authoring.goPrefab, TransformUsageFlags.Renderable | TransformUsageFlags.WorldSpace),
                Count = authoring.nPoolCount,
            });
        }
    }
}

public struct PokerPoolObj : IComponentData
{
    public Entity Prefab;
    public int Count;
}

