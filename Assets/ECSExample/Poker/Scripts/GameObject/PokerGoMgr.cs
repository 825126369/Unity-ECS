using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class PokerGoMgr : MonoBehaviour
{
    public GameObject startPt_obj;
    public GameObject TopLeft_obj;
    public GameObject TopRight_obj;
    public GameObject BottomLeft_obj;
    public GameObject BottomRight_obj;

    public void Start()
    {
        Debug.Log("Start");
        GetScreenCornersWorldPoints();
        EntityManager mEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity QueryEntity = mEntityManager.CreateEntity();
        mEntityManager.AddComponent<PokerSystemInitFinishCData>(QueryEntity);
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
        Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));

        TopLeft_obj.transform.position = topLeft;
        TopRight_obj.transform.position = topRight;
        BottomLeft_obj.transform.position = bottomLeft;
        BottomRight_obj.transform.position = bottomRight;
    }
}
