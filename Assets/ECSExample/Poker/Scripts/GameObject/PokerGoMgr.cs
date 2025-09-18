using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.U2D;

public class PokerGoMgr : SingleTonMonoBehaviour<PokerGoMgr>
{
    public SpriteAtlas mPokerAtlas;
    public SpriteAtlas mPokerBackAtlas;

    public Canvas mCanvas;
    public GameObject startPt_obj1;
    public GameObject startPt_obj2;
    public GameObject startPt_obj3;
    public GameObject startPt_obj4;

    public GameObject TopLeft_obj;
    public GameObject TopRight_obj;
    public GameObject BottomLeft_obj;
    public GameObject BottomRight_obj;

    public void Start()
    {
        Debug.Log("Start");
        GetScreenCornersWorldPoints();
        EntityManager mEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity QueryEntity = mEntityManager.CreateEntity(typeof(PokerGoMgrInitFinishEvent));
    }

    private Entity FindMyEntity()
    {
        EntityManager mEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entities = mEntityManager.CreateEntityQuery(typeof(PokerSystemCData)).ToEntityArray(Allocator.Temp);
        foreach (Entity entity in entities)
        {
            Debug.Log($"My Entity Find Ok");
            return entity;
        }

        return Entity.Null;
    }

    /// <summary>
    /// 获取屏幕四个角的世界坐标（可选）
    /// </summary>
    public void GetScreenCornersWorldPoints()
    {
        float w = Screen.width;
        float h = Screen.height;

        float depth = mCanvas.planeDistance;
        Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, depth));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth));
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, depth));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, depth));

        TopLeft_obj.transform.position = topLeft;
        TopRight_obj.transform.position = topRight;
        BottomLeft_obj.transform.position = bottomLeft;
        BottomRight_obj.transform.position = bottomRight;
    }
}
