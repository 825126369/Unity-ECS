using Unity.Entities;
using UnityEngine;

public struct PokerAnimationCData : IComponentData
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
    public float checktimes; //ÿ��֡�����һ��λ�á�
    public Vector3 startPt;  //�ʼ����ʼ�㡣
    public Vector3 nowPt;
    public float vx;    // x������ٶ�
    public float vy;    // y������ٶ�  
    public float vyMax; // y���������ٶ�
    public float vx_a;  // x����ļ��ٶ� x������
    public float vy_a;  // y����ļ��ٶ�
   // public GameObject firstNode;
    public float maxHeight;   //ÿ�θ������ֵ��


    public void Init()
    {
        index = 0; //�ڼ���col��
        value = 13;
        cardid = 1;  //13*v+value cardid
        color = 0;
        open = false;
        trigger = false;
        triggerDelay = 0;
        btoRight = true;
        deltTime = 0;
        checktimes = 0; //ÿ��֡�����һ��λ�á�

        startPt = new Vector3(0, 0, 0);  //�ʼ����ʼ�㡣
        nowPt = new Vector3(0, 0, 0);

        vx = 0;    // x������ٶ�
        vy = 0;    // y������ٶ�  
        vyMax = 0; // y���������ٶ�
        vx_a = 0;  // x����ļ��ٶ� x������
        vy_a = 0;  // y����ļ��ٶ�
        //firstNode = null!;
        maxHeight = 0;   //ÿ�θ������ֵ��
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

    public static PokerAnimationCData create(int index, int color, int value)
    {
        PokerAnimationCData entity = new PokerAnimationCData();
        entity.color = color;
        entity.index = index;
        entity.value = value;
        entity.cardid = PokerAnimationCData.getCardId(color, value);
        return entity;
    }
}