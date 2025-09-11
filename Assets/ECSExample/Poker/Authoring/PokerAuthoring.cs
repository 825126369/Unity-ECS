using Unity.Entities;
using UnityEngine;

//扑克牌 Image 烘培器
public class PokerAuthoring : MonoBehaviour
{
    class mBaker : Baker<PokerAuthoring>
    {
        public override void Bake(PokerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveParam());
            SetComponent(entity, new MoveParam() { x = 5 });
            AddComponent<PokerComponentData>(entity);
            AddComponent<PokerAnimationComponentData>(entity);
        }
    }
}

public struct PokerComponentData : IComponentData
{
    
}

public struct PokerAnimationComponentData : IComponentData
{
    public int index;
    public int value;
    public int cardid;
    public int color;
    public bool open;
    public bool trigger;
    public float triggerDelay;
    public bool btoRight;
    public float deltTime;
    public float checktimes; //每两帧，检查一次位置。
    public Vector3 startPt;  //最开始的起始点。
    public Vector3 nowPt;
    public float vx;    // x方向的速度
    public float vy;    // y方向的速度  
    public float vyMax; // y方向的最大速度
    public float vx_a;  // x方向的加速度 x轴匀速
    public float vy_a;  // y方向的加速度
    public GameObject firstNode;
    public float maxHeight;   //每次更新最高值。

    public void Init()
    {
        index = 0; //第几个col的
        value = 13;
        cardid = 1;  //13*v+value cardid
        color = 0;
        open = false;
        trigger = false;
        triggerDelay = 0;
        btoRight = true;
        deltTime = 0;
        checktimes = 0; //每两帧，检查一次位置。

        startPt = new Vector3(0, 0, 0);  //最开始的起始点。
        nowPt = new Vector3(0, 0, 0);

        vx = 0;    // x方向的速度
        vy = 0;    // y方向的速度  
        vyMax = 0; // y方向的最大速度
        vx_a = 0;  // x方向的加速度 x轴匀速
        vy_a = 0;  // y方向的加速度
        firstNode = null!;
        maxHeight = 0;   //每次更新最高值。
    }

    public static bool toRight(int index)
    {
        int[] toRightRate = new int[] { 100, 50, 40, 30 };
        int rate = UnityEngine.Random.Range(0, 100);
        bool toRight = rate <= toRightRate[index];
        return toRight;
    }

    public static float randomVx()
    {
        return UnityEngine.Random.Range(200, 260);
    }

    public static float randomVy()
    {
        return UnityEngine.Random.Range(500, 600);
    }

    public static float randomVy_a()
    {
        return UnityEngine.Random.Range(5500, 6200) * (-1);
    }

    public static int getCardId(int color, int value)
    {
        return (int)color * 13 + value;
    }

    public static PokerAnimationComponentData create(int index, int color, int value)
    {
        PokerAnimationComponentData entity = new PokerAnimationComponentData();
        entity.color = color;
        entity.index = index;
        entity.value = value;
        entity.cardid = PokerAnimationComponentData.getCardId(color, value);
        return entity;
    }
}

