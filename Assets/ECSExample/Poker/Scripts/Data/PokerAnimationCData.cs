using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PokerItemCData : IComponentData
{
    public int color;
    public int cardNum;
    public int nCardId;
}

public struct PokerAnimationCData : IComponentData
{
    public Entity mEntity;
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
    public float3 startPt;  //�ʼ����ʼ�㡣
    public float3 nowPt;
    public float vx;    // x������ٶ�
    public float vy;    // y������ٶ�  
    public float vyMax; // y���������ٶ�
    public float vx_a;  // x����ļ��ٶ� x������
    public float vy_a;  // y����ļ��ٶ�

    public float maxHeight;
    public float minHeight;
    public float maxWidth;
    public float minWidth;

    public void Reset()
    {
        mEntity = Entity.Null;
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
        maxHeight = 0;   //ÿ�θ������ֵ��
    }

    public void Init(int index, int color, int value)
    {
        this.color = color;
        this.index = index;
        this.value = value;
        this.cardid = PokerAnimationCData.getCardId(color, value);
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
        return UnityEngine.Random.Range(300, 500);
    }

    public static float randomVy_a()
    {
        return UnityEngine.Random.Range(4000, 5000) * -1;
    }

    public static int getCardId(int color, int value)
    {
        return (int)color * 13 + value;
    }

}