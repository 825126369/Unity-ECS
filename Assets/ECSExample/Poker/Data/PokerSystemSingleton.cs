using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum PokerGameState
{
    Start = 0,
    Playing = 1,
    End = 2,
}

public struct CardAnimationItem : IComponentData
{
    public float3 worldPos_start;
    public float3 worldPos_ScreenTopLeft;
    public float3 worldPos_ScreenBottomRight;
    public PokerGameState State;
}

public struct PokerSystemSingleton : IComponentData
{
    public float3 worldPos_start;
    public float3 worldPos_ScreenTopLeft;
    public float3 worldPos_ScreenBottomRight;
    public PokerGameState State;

    // Card 数据
    //public const float CardWidth = 103;
    //public const float CardHeigt = 154;
    //public const float CardsColTotal = 120; //每个数字最多显示多少张card，避免过多导致卡顿。
    //public const float Delay_Col_Offset = 0.1f; //5*2/30
    //public const float Delay_Value_Offset = 0.01f; // 22*2/30

    public Entity skipNode;
    public Entity cardsNode;

    public float2 animationSize;
    public float maxHeight;
    public float minHeight;
    public float maxWidth;
    public float minWidth;
    public NativeHashMap<int, NativeList<CardAnimationItem>> colNodes_Dic;
    public NativeList<CardAnimationItem> allNodes;
    public NativeList<PokerAnimationCData> animationEntitys;
    public bool animationOver;
    public NativeArray<int> colors;

    //public void Init()
    //{
    //    worldPos_start = float3.zero;
    //    worldPos_ScreenTopLeft = float3.zero;
    //    worldPos_ScreenBottomRight = float3.zero;
    //    State = PokerGameState.Start;
    //    skipNode = Entity.Null;
    //    animationSize = float2.zero;
    //    maxHeight = 0;
    //    minHeight = 0;
    //    maxWidth = 0;
    //    minWidth = 0;
    //    colNodes_Dic = new NativeHashMap<int, NativeList<CardAnimationItem>>();
    //    allNodes = new NativeList<CardAnimationItem>();
    //    animationEntitys = new NativeList<PokerAnimationCData>();
    //    animationOver = false;
    //    cardsNode = null;
    //    colors = new NativeArray<int>();
    //}

}