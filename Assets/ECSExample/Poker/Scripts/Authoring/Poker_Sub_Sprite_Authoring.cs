using Unity.Entities;
using UnityEngine;

//ÆË¿ËÅÆ Image ºæÅàÆ÷
public class Poker_Sub_Sprite_Authoring : MonoBehaviour
{
    class mBaker : Baker<Poker_Sub_Sprite_Authoring>
    {
        public override void Bake(Poker_Sub_Sprite_Authoring authoring)
        {
            var mTargetEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Material_Color_CData>(mTargetEntity);
            AddComponent<Material_MainTex_CData>(mTargetEntity);
            AddComponent<Material_MainTex_ST_CData>(mTargetEntity);
        }
    }
}

