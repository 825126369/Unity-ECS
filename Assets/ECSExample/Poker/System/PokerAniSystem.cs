//using System;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//[BurstCompile]
//public partial class PokerAniSystem : SystemBase
//{
//    protected override void OnCreate()
//    {
//        base.OnCreate();
//    }

//    protected override void OnDestroy()
//    {
//        base.OnDestroy();
//    }

//    protected override void OnStartRunning()
//    {
//        base.OnStartRunning();
//        //如果系统中途被 Enabled = false，再设为 true，它会再次调用； 类似 MonoBehaviour.Enable
//    }

//    protected override void OnStopRunning()
//    {
//        base.OnStopRunning();
//    }

//    protected override void OnCreateForCompiler()
//    {
//        base.OnCreateForCompiler();
//    }

//    protected override void OnUpdate()
//    {
//        float deltaTime = SystemAPI.Time.DeltaTime;
//        PokerSystemSingleton mGlobalData = SystemAPI.GetSingleton<PokerSystemSingleton>();
//        if(mGlobalData.State == PokerGameState.Start)
//        {
//            Show();
//        }
//        else if(mGlobalData.State == PokerGameState.Playing)
//        {
//            Entities.ForEach((ref LocalTransform transform, ref PokerAnimationCData obj) =>
//            {
//                this.updateAnimation(obj, deltaTime);

//            }).ScheduleParallel();
//        }
//        else if(mGlobalData.State == PokerGameState.End)
//        {
//            UnityMainThreadDispatcher.Instance.Enqueue(new MainThreadData_End());
//        }
//        else
//        {
//            Unity.Assertions.Assert.IsTrue(false);
//        }
//    }

//    void Awake()
//    {
//        RectTransform tUITransform = GameLauncher.Instance.mUIRoot.mCanvas_Tip;
//        this.animationSize = new Vector2(tUITransform.rect.width, tUITransform.rect.height);

//        this.maxHeight = this.animationSize.y / 2;
//        this.minHeight = -this.animationSize.y / 2 + 100;
//        this.maxWidth = this.animationSize.x / 2 + CardWidth;
//        this.minWidth = -this.animationSize.x / 2 - CardWidth;

//        this.maxWidth /= GameLauncher.Instance.mUIRoot.mCanvas_WinAnimation.localScale.x;
//        this.minWidth /= GameLauncher.Instance.mUIRoot.mCanvas_WinAnimation.localScale.x;

//        this.cardsNode = this.transform.FindDeepChild("cardsnode").GetComponent<RectTransform>();
//        this.cardsNode.gameObject.removeAllChildren();
//    }

//    public void Show(NativeArray<int> colors, float3 startPt_w, int offsetX, Action callback)
//    {
//        this.animationOver = false;
//        this.skipNode.SetActive(true);
//        this.colors = new NativeArray<int>();
//        this.colors.add(element);
//        this.callBack = callback;
//        this.showAnimation(colors, startPt_w, offsetX);
//    }

//    void showAnimation(ref NativeList<int> colors, float3 startPt_w, int offsetX = 0)
//    {
//        Vector3 firstPt_l = GameTools.WorldToUILocalPos(startPt_w, this.cardsNode);
//        this.showAnimation_Default(firstPt_l, colors, offsetX);
//    }
    
//    // 默认弹跳动画。
//    void showAnimation_Default(Vector3 pt, TSArray<CardType> colors, int offsetX)
//    {
//        float delay = 0.1f;
//        for (int index = 0; index < colors.length; index++)
//        {
//            CardType color = colors[index];
//            int offset = offsetX * index;
//            Vector3 frompt = new Vector3(pt.x + index * (CardWidth - 1) + offset, pt.y, pt.z);
//            delay = index * 0.5f; //+ 0.1;  
//            this.showAnimation_Default_Col(index, frompt, delay, color, offsetX);
//        }
//    }

//    // 每一个位置的动画
//    void showAnimation_Default_Col(int colindex, Vector3 pt, float delay, CardType color, int offsetX = 0)
//    {
//        float delayvalue = 0;
//        float delayoffset = 0.1f;
//        for (int index = 13; index > 0; index--)
//        {
//            this.showAnimation_Default_ColValue(colindex, pt, delay + delayvalue, color, index, offsetX);
//            delayvalue += delayoffset;
//        }
//    }


//    // 每一列中，单个数字的移动。
//    void showAnimation_Default_ColValue(int colindex, Vector3 pt, float delay, CardType color, int value, int offsetX = 0)
//    {
//        (GameObject node, bool) ret = this.addStaticCard(pt, color, value, colindex);
//        var startNode = ret.node;

//        var entity = AnimationEntity.create(colindex, color, value);
//        entity.open = true;
//        entity.trigger = false;
//        entity.triggerDelay = delay;
//        entity.btoRight = AnimationEntity.toRight(colindex);
//        entity.startPt = pt;
//        entity.nowPt = pt;
//        entity.firstNode = startNode;
//        entity.maxHeight = this.maxHeight;

//        entity.vx = AnimationEntity.randomVx();
//        entity.vx_a = 0;

//        entity.vy = AnimationEntity.randomVy();
//        if (!entity.btoRight)
//        {
//            entity.vx *= -1;
//        }

//        entity.vy_a = AnimationEntity.randomVy_a();
//        entity.deltTime = 0;
//        entity.firstNode = startNode;
//        this.animationEntitys.push(entity);
//        startNode.transform.SetSiblingIndex(0);

//    }

//    // 检查是否最后一个队列中的最后一个，标志动画结束。   
//    (GameObject, bool) addStaticCard(Vector3 pt, CardType colorType, int value, int colindex)
//    {
//        int nodekey = AnimationEntity.getCardId(colorType, value);
//        TSArray<CardAnimationItem> nodeArrs = this.colNodes_Dic.get(nodekey);
//        if (nodeArrs == null)
//        {
//            nodeArrs = new TSArray<CardAnimationItem>();
//            this.colNodes_Dic.set(nodekey, nodeArrs);
//        }

//        bool firstNodeByNewValue = false;
//        if (value == 13 && nodeArrs.length == 0)
//        {
//            firstNodeByNewValue = true;
//        }

//        AnimationEntity entity = null;
//        // 从后往前找，找到第一个。
//        for (int index = this.animationEntitys.length - 1; index >= 0; index--)
//        {
//            AnimationEntity element = this.animationEntitys[index];
//            if (element.color == colorType && element.value == value && element.index == colindex)
//            {
//                entity = element;
//                break;
//            }
//        }

//        if (nodeArrs.length >= CardsColTotal)
//        {
//            if (entity == null)
//            {

//            }
//            else
//            {
//                if (entity.color == this.colors[this.colors.length - 1] && entity.value == 1)
//                {
//                    this.onAnimatinCallBack();
//                    this.DoDestroyAction();
//                }

//            }
//            return (null, firstNodeByNewValue);
//        }

//        CardAnimationItem startNode = ResCenter.Instance.mCardAnimationItemPool.popObj();
//        startNode.transform.SetParent(this.cardsNode.transform, false);
//        startNode.transform.localPosition = pt;
//        nodeArrs.push(startNode);

//        startNode.initByNum(value, colorType);

//        this.allNodes.push(startNode);
//        return (startNode.gameObject, firstNodeByNewValue);
//    }
    
//    void updateAnimation(PokerAnimationCData entity, LocalTransform lt, float dt)
//    {
//        if (!entity.open)
//        {
//            return;
//        }

//        if (entity.trigger)
//        {
//            // 没有节点的时候，不更新。
//            if (entity.firstNode == null)
//            {
//                return;
//            }

//            var deltTime = entity.deltTime;
//            var startPt = entity.nowPt;
//            var firstNode = entity.firstNode;
//            var maxHeight = entity.maxHeight;
//            var toRight = entity.btoRight;
//            var vx_a = entity.vx_a;
//            var vy_a = entity.vy_a;

//            var nowPt = new Vector3(0, 0, 0);

//            // 匀变速直线运动位移公式：a=dv/dt，
//            // 距离 x = v0t+1/2·at^2

//            // 现在速度
//            entity.vx += vx_a * dt;
//            entity.vy += vy_a * dt;

//            var vx = entity.vx;
//            var vy = entity.vy;

//            nowPt.x = (float)(startPt.x + vx * dt + 0.5f * vx_a * dt * dt);
//            nowPt.y = (float)(startPt.y + vy * dt + 0.5f * vy_a * dt * dt);
//            nowPt.z = startPt.z;

//            // 垂直. 小于最低值。
//            if (nowPt.y < this.minHeight)
//            {
//                nowPt.y = this.minHeight;
//                entity.vy *= -0.95f;  //转变方向
//            }

//            if (nowPt.y > entity.maxHeight)
//            {
//                nowPt.y = entity.maxHeight;
//                // vy_a = -vy_a;
//                entity.vy = 0;
//                entity.maxHeight = entity.maxHeight * 0.8f;
//            }


//            // X轴移除就消失。
//            bool willRemove = false;
//            // 水平 不会碰撞后返回了。
//            if (nowPt.x < this.minWidth - CardWidth)
//            {
//                nowPt.x = this.minWidth;
//                willRemove = true;
//            }

//            if (nowPt.x > this.maxWidth + CardWidth)
//            {
//                nowPt.x = this.maxWidth;
//                entity.vx *= -1;
//                willRemove = true;


//            }

//            // 每两帧之间 添加
//            entity.checktimes += 1;
//            firstNode.transform.localPosition = nowPt;
//            entity.nowPt = nowPt;

//            // 碰到边界，不在产生新的额
//            if (entity.open)
//            {
//                firstNode.transform.localPosition = nowPt;
//                if (willRemove)
//                {
//                    entity.open = false;
//                    if (entity.value == 6 && entity.index == 2)
//                    {
//                        this.onAnimatinCallBack();
//                        this.DoDestroyAction();
//                    }
//                    return;
//                }
//            }
//        }
//        else
//        {

//            entity.triggerDelay -= dt;
//            if (entity.triggerDelay <= 0)
//            {
//                entity.trigger = true;
//            }
//        }
//    }

//    void onAnimatinCallBack()
//    {
//        if (this.callBack != null)
//        {
//            this.callBack();
//            this.callBack = null;
//        }
//    }

//    public void DoDestroyAction()
//    {
//        if (this.animationOver)
//        {
//            return;
//        }

//        this.animationOver = true;
//        for (int index = 0; index < this.animationEntitys.length; index++)
//        {
//            AnimationEntity element = this.animationEntitys[index];
//            element.open = false;
//        }
//        this.animationEntitys = new TSArray<AnimationEntity>();

//        foreach (var v in this.allNodes)
//        {
//            ResCenter.Instance.mCardAnimationItemPool.recycleObj(v);
//        }
//        this.allNodes.Clear();
//        Destroy(gameObject);
//    }

//    public void onClick_Skip()
//    {
//        this.skipNode.SetActive(false);
//        AudioController.Instance.playSound(Sounds.button, 1);
//        this.onAnimatinCallBack();
//        this.DoDestroyAction();
//        TAController.Instance.trackAnimationSkip();
//    }


//}