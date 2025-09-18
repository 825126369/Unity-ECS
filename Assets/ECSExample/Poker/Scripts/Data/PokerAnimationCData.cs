using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D;

public struct PokerItemCData : IComponentData
{
    public UnityObjectRef<GameObject> n_root;
    public UnityObjectRef<SpriteRenderer> n_card;
    public UnityObjectRef<SpriteRenderer> n_back;
    public int nCardId;

    public void initByNum(int cardNum, int colorType)
    {
        this.nCardId = (int)colorType * 13 + cardNum;

        //ˢ�»�ɫ����ui
        SpriteAtlas atl_game = PokerGoMgr.Instance.mPokerAtlas;

        string p_name = "di_" + cardNum + "_" + colorType;
        Sprite spri_bg = atl_game.GetSprite(p_name);
        n_card.Value.sprite = spri_bg;

        SpriteAtlas atl_game1 = PokerGoMgr.Instance.mPokerBackAtlas;
        string p_name_back = "cardback_1";
        Sprite spri_bg_back = atl_game1.GetSprite(p_name_back);
        n_back.Value.sprite = spri_bg_back;

        this.onSetNormal();
    }

    void onSetBack()
    {
        this.n_card.Value.gameObject.SetActive(false);
        this.n_back.Value.gameObject.SetActive(true);
    }

    void onSetNormal()
    {
        this.n_back.Value.gameObject.SetActive(false);
        this.n_card.Value.gameObject.SetActive(true);
    }
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