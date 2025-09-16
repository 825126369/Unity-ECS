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

        ////ˢ�»�ɫ����ui
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
    public float checktimes; //ÿ��֡�����һ��λ�á�
    public Vector3 startPt;  //�ʼ����ʼ�㡣
    public Vector3 nowPt;
    public float vx;    // x������ٶ�
    public float vy;    // y������ٶ�  
    public float vyMax; // y���������ٶ�
    public float vx_a;  // x����ļ��ٶ� x������
    public float vy_a;  // y����ļ��ٶ�
    public float maxHeight;   //ÿ�θ������ֵ��
    
    public void Reset()
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