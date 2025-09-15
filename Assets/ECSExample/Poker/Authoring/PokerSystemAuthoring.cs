using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

//�˿��� Image ������
public class PokerSystemAuthoring : MonoBehaviour
{
    public Image startPt_obj;
    class mBaker : Baker<PokerSystemAuthoring>
    {
        public override void Bake(PokerSystemAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            //(Vector3 topLeft, Vector3 bottomRight) = authoring.GetScreenCornersWorldPoints();
            //var mData = new PokerSystemCData();
            //mData.worldPos_start = authoring.startPt_obj.transform.position;
            //mData.worldPos_ScreenTopLeft = topLeft;
            //mData.worldPos_ScreenBottomRight = bottomRight;
            //AddComponent(entity, mData);
        }
    }
    
    /// <summary>
    /// ��ȡ��Ļ�ĸ��ǵ��������꣨��ѡ��
    /// </summary>
    public (Vector3 topLeft, Vector3 bottomRight) GetScreenCornersWorldPoints()
    {
        float w = Screen.width;
        float h = Screen.height;
        Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        return (topLeft, bottomRight);
    }

}

