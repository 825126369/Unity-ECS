using Unity.Entities;
using UnityEngine;

public struct PokerItemCData : IComponentData
{
    //public SpriteRenderer n_card = null;
    //public SpriteRenderer n_back = null;
    //public GameObject n_root = null;
    //public CardType cardHuase = CardType.FangPian;
    //public int cardDianshu = 0;
    //public int nCardId = 0;

    public void initByNum(int cardNum, int colorType)
    {
        //this.cardDianshu = cardNum;
        //this.cardHuase = colorType;
        //this.nCardId = ((int)this.cardHuase - 1) * 13 + this.cardDianshu;

        ////刷新花色点数ui
        //Image sp_ = this.n_card;
        //int id = DataCenter.Instance.sysConfig.themeZhengId;
        //string path = "cards" + id;
        //SpriteAtlas atl_game = ResCenter.Instance.mBundleGameAllRes.GetAtlas(path);

        //string p_name = "di_" + this.cardDianshu + "_" + (int)this.cardHuase;
        //Sprite spri_bg = atl_game.GetSprite(p_name);
        //PrintTool.Assert(spri_bg != null, p_name);
        //sp_.sprite = spri_bg;

        //SpriteAtlas atl_game1 = ResCenter.Instance.mBundleGameAllRes.GetAtlas(AtlasNames.Lobby_Game_Cards_Cardback);
        //Image sp_back = this.n_back;
        //string p_name_back = "cardback_" + DataCenter.Instance.sysConfig.themeBackId;
        //Sprite spri_bg_back = atl_game1.GetSprite(p_name_back);
        //sp_back.sprite = spri_bg_back;

        //this.onSetNormal();
    }

    void onSetBack()
    {
        //this.n_card.gameObject.SetActive(false);
        //this.n_back.gameObject.SetActive(true);
    }

    void onSetNormal()
    {
        //this.n_back.gameObject.SetActive(false);
        //this.n_card.gameObject.SetActive(true);
    }
}

public class PokerAnimationCData : IComponentData
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
    public Vector3 startPt;  //最开始的起始点。
    public Vector3 nowPt;
    public float vx;    // x方向的速度
    public float vy;    // y方向的速度  
    public float vyMax; // y方向的最大速度
    public float vx_a;  // x方向的加速度 x轴匀速
    public float vy_a;  // y方向的加速度
    public float maxHeight;   //每次更新最高值。
    
    public void Reset()
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
        //firstNode = null!;
        maxHeight = 0;   //每次更新最高值。
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

}