using Unity.Entities;
using UnityEngine;

//�˿��� Image ������
public class PokerAuthoring : MonoBehaviour
{
    class mBaker : Baker<PokerAuthoring>
    {
        public override void Bake(PokerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PokerAnimationCData>(entity);
            AddComponent<PokerItemCData>(entity);
        }
    }
}

