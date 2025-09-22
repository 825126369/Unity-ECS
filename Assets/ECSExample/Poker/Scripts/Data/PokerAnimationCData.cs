using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PokerItemCData : IComponentData
{
    public int color;
    public int cardNum;
    public int nCardId;
}

public struct PokerTimerRemoveCData : IComponentData
{
    public float mRomveCdTime;
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
    public float checktimes; //每两帧，检查一次位置。
    public float3 startPt;  //最开始的起始点。
    public float3 nowPt;
    public float vx;    // x方向的速度
    public float vy;    // y方向的速度  
    public float vyMax; // y方向的最大速度
    public float vx_a;  // x方向的加速度 x轴匀速
    public float vy_a;  // y方向的加速度

    public float maxHeight;
    public float minHeight;
    public float maxWidth;
    public float minWidth;
    
    public void Reset()
    {
        mEntity = Entity.Null;
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
        maxHeight = 0;   //每次更新最高值。
    }

    public void Init(int index, int color, int value)
    {
        this.color = color;
        this.index = index;
        this.value = value;
        this.cardid = getCardId(color, value);
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
        return UnityEngine.Random.Range(100, 200);
    }

    public static float randomVy()
    {
        return UnityEngine.Random.Range(100, 300);
    }

    public static float randomVy_a()
    {
        return UnityEngine.Random.Range(2000, 3000) * -1;
    }

    public static int getCardId(int color, int value)
    {
        return (int)color * 13 + value;
    }

}

public struct PokerAnimationCData2 : IComponentData
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
    public float checktimes; //每两帧，检查一次位置。
    public float3 startPt;  //最开始的起始点。
    public float3 nowPt;
    public float vx;    // x方向的速度
    public float vy;    // y方向的速度  
    public float vyMax; // y方向的最大速度
    public float vx_a;  // x方向的加速度 x轴匀速
    public float vy_a;  // y方向的加速度

    public float maxHeight;
    public float minHeight;
    public float maxWidth;
    public float minWidth;

    public void Reset()
    {
        mEntity = Entity.Null;
        index = 0; //第几个col的
        value = 13;
        cardid = 1;  //13*v+value cardid
        color = 0;
        open = false;
        trigger = false;
        triggerDelay = 0;
        btoRight = true;
        checktimes = 0; //每两帧，检查一次位置。

        startPt = new Vector3(0, 0, 0);  //最开始的起始点。
        nowPt = new Vector3(0, 0, 0);

        vx = 0;    // x方向的速度
        vy = 0;    // y方向的速度  
        vyMax = 0; // y方向的最大速度
        vx_a = 0;  // x方向的加速度 x轴匀速
        vy_a = 0;  // y方向的加速度
        maxHeight = 0;   //每次更新最高值。
    }

    public void Init(int index, int color, int value)
    {
        this.color = color;
        this.index = index;
        this.value = value;
        this.cardid = getCardId(color, value);
    }

    public static bool toRight()
    {
        bool toRight = UnityEngine.Random.Range(0, 100) < 100;
        return toRight;
    }

    public static float randomVx()
    {
        return UnityEngine.Random.Range(50, 200);
    }

    public static float randomVy()
    {
        return UnityEngine.Random.Range(100, 150);
    }

    public static float randomVy_a()
    {
        return UnityEngine.Random.Range(1000, 1500) * -1;
    }

    public static int getCardId(int color, int value)
    {
        return (int)color * 13 + value;
    }

}