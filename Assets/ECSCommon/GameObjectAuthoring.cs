using Unity.Entities;
using UnityEngine;

//ÆË¿ËÅÆ Image ºæÅàÆ÷
public class GameObjectAuthoring : MonoBehaviour
{
    class mBaker : Baker<GameObjectAuthoring>
    {
        public override void Bake(GameObjectAuthoring authoring)
        {
            Entity mEntity = GetEntity(TransformUsageFlags.Dynamic);
            var mGameObjectCData = new GameObjectCData();
            mGameObjectCData.Init(authoring.gameObject);
            AddComponent(mEntity, mGameObjectCData);

        }
    }
}

