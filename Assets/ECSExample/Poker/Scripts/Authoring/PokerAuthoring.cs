using Unity.Entities;
using UnityEngine;

//ÆË¿ËÅÆ Image ºæÅàÆ÷
public class PokerAuthoring : MonoBehaviour
{
    class mBaker : Baker<PokerAuthoring>
    {
        public override void Bake(PokerAuthoring authoring)
        {
            //ECS_Authoring_Helper.AddCData(this, authoring.gameObject);
            //ECS_Authoring_Helper.AddCData<SpriteRenderer, SpriteRendererCData>(this, authoring.gameObject);
            var entity = GetEntity(TransformUsageFlags.Dynamic);
        }
    }
}

