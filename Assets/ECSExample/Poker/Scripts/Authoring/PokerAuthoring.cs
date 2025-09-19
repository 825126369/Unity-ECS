using Unity.Entities;
using UnityEngine;

//�˿��� Image ������
public class PokerAuthoring : MonoBehaviour
{
    public GameObject root;
    public SpriteRenderer Poker;
    public SpriteRenderer Back;

    class mBaker : Baker<PokerAuthoring>
    {
        public override void Bake(PokerAuthoring authoring)
        {
            //ECS_Authoring_Helper.AddCData(this, authoring.gameObject);
            //ECS_Authoring_Helper.AddCData<SpriteRenderer, SpriteRendererCData>(this, authoring.gameObject);

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PokerAnimationCData>(entity);
            AddComponent<PokerItemCData>(entity);
        }
    }
}

