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

public enum CardType
{
    MeiHua = 1,
    HongTao = 2,
    FangPian = 3,
    HeiTao = 4
}

public struct PokerSystemInitFinishCData : IComponentData
{

}

public struct PokerSystemCData : IComponentData
{
    //public UnityObjectRef<GameObject> startPt_obj;
    //public UnityObjectRef<GameObject> TopLeft_obj;
    //public UnityObjectRef<GameObject> TopRight_obj;
    //public UnityObjectRef<GameObject> BottomLeft_obj;
    //public UnityObjectRef<GameObject> BottomRight_obj;
}

public struct PokerSystemSingleton : IComponentData, IDisposable
{
    public float3 worldPos_start;
    public float3 worldPos_ScreenTopLeft;
    public float3 worldPos_ScreenBottomRight;
    public PokerGameState State;

    // Card ����
    public const float CardWidth = 103;
    public const float CardHeigt = 154;
    public const int CardsColTotal = 120; //ÿ�����������ʾ������card��������ർ�¿��١�
    public const float Delay_Col_Offset = 0.1f; //5*2/30
    public const float Delay_Value_Offset = 0.01f; // 22*2/30

    public Entity Prefab;
    public Entity cardsNode;
    
    public float2 animationSize;
    public float maxHeight;
    public float minHeight;
    public float maxWidth;
    public float minWidth;
    public bool animationOver;

    public NativeHashMap<int, NativeList<Entity>> colNodes_Dic;
    public NativeList<Entity> allNodes;
    public NativeList<Entity> animationEntitys;
    public NativeArray<int> colors;
    //public UnityObjectRef<Action> callBack;

    private bool bDispose;
    public void Dispose()
    {
        if (bDispose) return; bDispose = true;
        if (colors.IsCreated)
        {
            colors.Dispose();
        }
        if (colNodes_Dic.IsCreated)
        {
            colNodes_Dic.Dispose();
        }
        if (allNodes.IsCreated)
        {
            allNodes.Dispose();
        }
        if (animationEntitys.IsCreated)
        {
            animationEntitys.Dispose();
        }
    }
}