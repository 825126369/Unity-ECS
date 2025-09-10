using Unity.Entities;
using UnityEngine;

//ÆË¿ËÅÆ Image ºæÅàÆ÷
public class PokerAuthoring : MonoBehaviour
{
    public class mBaker : Baker<PokerAuthoring>
    {
        public override void Bake(PokerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new MoveParam());
            SetComponent(entity, new MoveParam() { x = 5 });
            AddComponent(entity, new PokerObj
            {
                Prefab = GetEntity(authoring.gameObject, TransformUsageFlags.Renderable | TransformUsageFlags.WorldSpace),
                Count = 100000,
                InitialRadius = 9999
            });
        }
    }
}

public struct PokerObj : IComponentData
{
    public Entity Prefab;
    public float InitialRadius;
    public int Count;
}

