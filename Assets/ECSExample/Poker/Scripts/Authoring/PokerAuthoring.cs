using Unity.Entities;
using UnityEngine;

//ÆË¿ËÅÆ Image ºæÅàÆ÷
public class PokerAuthoring : MonoBehaviour
{
    public GameObject root;
    public SpriteRenderer Poker;
    public SpriteRenderer Back;

    class mBaker : Baker<PokerAuthoring>
    {
        public override void Bake(PokerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PokerAnimationCData>(entity);

            PokerItemCData mItem = new PokerItemCData();
            mItem.n_root = authoring.root;
            mItem.n_card = authoring.Poker;
            mItem.n_back = authoring.Back;
            AddComponent(entity, mItem);
        }
    }
}

