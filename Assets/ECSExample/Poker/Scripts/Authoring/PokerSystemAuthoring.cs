using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

//扑克牌 Image 烘培器
public class PokerSystemAuthoring : MonoBehaviour
{
    class mBaker : Baker<PokerSystemAuthoring>
    {
        public override void Bake(PokerSystemAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var mData = new PokerSystemCData();
            //mData.startPt_obj = authoring.startPt_obj;
            //mData.TopLeft_obj = authoring.TopLeft_obj;
            //mData.TopRight_obj = authoring.TopRight_obj;
            //mData.BottomLeft_obj = authoring.BottomLeft_obj;
            //mData.BottomRight_obj = authoring.BottomRight_obj;
            AddComponent(entity, mData);
        }
    }
}

