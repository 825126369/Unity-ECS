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

public struct PokerSystemCData : IComponentData
{
    public float3 worldPos_start;
    public float3 worldPos_ScreenTopLeft;
    public float3 worldPos_ScreenBottomRight;
}

public struct PokerSystemSingleton : IComponentData, IDisposable
{
    public float3 worldPos_start;
    public float3 worldPos_ScreenTopLeft;
    public float3 worldPos_ScreenBottomRight;
    public PokerGameState State;

    // Card 数据
    public const float CardWidth = 103;
    public const float CardHeigt = 154;
    public const int CardsColTotal = 120; //每个数字最多显示多少张card，避免过多导致卡顿。
    public const float Delay_Col_Offset = 0.1f; //5*2/30
    public const float Delay_Value_Offset = 0.01f; // 22*2/30

    public Entity Prefab;
    public Entity cardsNode;
    
    public float2 animationSize;
    public float maxHeight;
    public float minHeight;
    public float maxWidth;
    public float minWidth;
    public NativeHashMap<int, NativeList<Entity>> colNodes_Dic;
    public NativeList<Entity> allNodes;
    public NativeList<Entity> animationEntitys;
    public bool animationOver;
    public NativeArray<int> colors;
    //public UnityObjectRef<Action> callBack;

    private bool bDispose;
    public void Dispose()
    {
        if (bDispose) return; bDispose = true;
        colors.Dispose();
    }

    public void Init()
    {
        //RectTransform tUITransform = GameLauncher.Instance.mUIRoot.mCanvas_Tip;
        //this.animationSize = new Vector2(tUITransform.rect.width, tUITransform.rect.height);

        //this.maxHeight = this.animationSize.y / 2;
        //this.minHeight = -this.animationSize.y / 2 + 100;
        //this.maxWidth = this.animationSize.x / 2 + CardWidth;
        //this.minWidth = -this.animationSize.x / 2 - CardWidth;

        //this.maxWidth /= GameLauncher.Instance.mUIRoot.mCanvas_WinAnimation.localScale.x;
        //this.minWidth /= GameLauncher.Instance.mUIRoot.mCanvas_WinAnimation.localScale.x;

        //this.cardsNode = this.transform.FindDeepChild("cardsnode").GetComponent<RectTransform>();
        //this.cardsNode.gameObject.removeAllChildren();
    }

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