using Unity.Entities;
using UnityEngine;

//ÆË¿ËÅÆ Image ºæÅàÆ÷
public class PokerAuthoring : MonoBehaviour
{
    public class PokerAuthoringBake : Baker<PokerAuthoring>
    {
        public override void Bake(PokerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            //AddComponent(entity, new MoveParam());
            //SetComponent(entity, new MoveParam() { x = 5 });
        }
    }
}

