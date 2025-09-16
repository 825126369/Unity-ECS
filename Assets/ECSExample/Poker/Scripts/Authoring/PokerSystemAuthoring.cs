using Unity.Entities;
using UnityEngine;

//扑克牌 Image 烘培器
public class PokerSystemAuthoring : MonoBehaviour
{
    class mBaker : Baker<PokerSystemAuthoring>
    {
        public override void Bake(PokerSystemAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            //NativeList 在 PokerSystemSingleton 中，这里会报错
            //AddComponent<PokerSystemSingleton>(entity);
        }
    }
}

