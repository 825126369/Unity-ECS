using Unity.Entities;
using UnityEngine;

public class SpriteRendererAuthoring : MonoBehaviour
{
    class mBaker : Baker<SpriteRendererAuthoring>
    {
        public override void Bake(SpriteRendererAuthoring authoring)
        {
            Entity mEntity = GetEntity(TransformUsageFlags.Dynamic);
            var mSpriteRendererCData = new SpriteRendererCData();
            mSpriteRendererCData.Init(authoring.gameObject);
            AddComponent(mEntity, mSpriteRendererCData);
        }
    }
}

