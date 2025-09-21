using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public enum PokerAniType
{
    Default = 0,
    FlyFullScreen = 1,
    FlyFullScreen2 = 2,
    FlyFullScreen3 = 3,
}

public enum PokerGameState
{
    None = 0,
    Start = 1,
    Playing = 2,
    End = 3,
}

public enum CardType
{
    MeiHua = 1,
    HongTao = 2,
    FangPian = 3,
    HeiTao = 4
}

public struct PokerGoMgrInitFinishEvent : IComponentData
{

}

public struct StartDoAniEvent : IComponentData
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
    public NativeArray<float3> worldPos_start_list;
    public NativeArray<float3> worldScale_start_list;
    public float3 worldPos_ScreenTopLeft;
    public float3 worldPos_ScreenBottomRight;
    public PokerGameState State;
    public PokerAniType nAniType;
    public float maxHeight;
    public float minHeight;
    public float maxWidth;
    public float minWidth;
    public bool animationOver;

    // Card 数据
    public const float CardWidth = 103;
    public const float CardHeigt = 154;
    public const int CardsColTotal = 120; //每个数字最多显示多少张card，避免过多导致卡顿。
    public const float Delay_Col_Offset = 0.1f; //5*2/30
    public const float Delay_Value_Offset = 0.01f; // 22*2/30

    public Entity Prefab;
    public Entity cardsNode;

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