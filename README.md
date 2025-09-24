# Unity-ECS-空当接龙-胜利动画-教程

UI Toolkit: (非必要)
(1) 得加上 InputSystemUIInputModule 这个组件，否则 Button 点击无响应
(2) VisualElement Display选项，得设置Display打开，否则在UI Builder中，无法可视化预览效果。
(3) PanelSettings 中 Clear Color 勾选得去掉，否则会黑屏

DOTS 的基础设施：
1： URP(非必要), UI Toolkit(非必要), New Input System(非必要), Entities, Entities Graphic,   ECS 得务必首先把这些 组件加到项目中来。

2：SubScene 
(1) SubScene 是 Authoring(创作) 的容器/舞台/编辑器，顾名思义: SubScene 就是 在GameObject世界里进行创作，通过在编译阶段烘培为 Entity世界。游戏启动后，创作阶段的GameObject世界全都会被Skip掉. 所以 SubScene 下面的所有GameObject 在运行阶段都不会执行 Awake, Start 这些生命周期函数，因为 这些GameObject 都被忽略掉了。
(2) Authoring 脚本（进行烘培Bake的脚本）只局限于 GameObject 本身， 不能在脚本中对他的子物体进行遍历添加IComponentData。 比如这个PokerItemObj，得专门在他的每个需要操作的子节点上 都加上相应的 Authoring 脚本。
(3) 错误的添加SubScene操作，会导致Entity没渲染出来, 看不见。  SubScene 需要点击 New Sub Scene 创建，在GameObjct上手动添加 SubScene脚本 会有问题。

3：实际ECS 逻辑：空当接龙案例
(1) OnUpdate  一帧内可能被调度了多次（查询变更、内部触发) 
(2) -- 烘培的实体和动态创建的实体 加载的时机不一样，这也导致了OnUpdate的触发时机，有可能是第一帧，也有可能是第三帧, 或其他帧。 PokerSystemInitFinishCData 和  PokerPoolCData 分别是 两类不同的实体，他们的加载时机不一样
    -- 总结 初始化系统的时候，得检查 初始系统状态是否 就绪/正确。 如下：

    protected override void OnUpdate()
    {
        Debug.Log($"OnUpdate 被调用，Enabled = {Enabled}");
        Debug.Log($"PokerSystemInitSystem OnUpdate - Frame: {UnityEngine.Time.frameCount}");

        //等待 信号完成，执行后面的
        //这个代码块 会导致 下面的组件 在前几次调用 OnUpdate 时，返回 null, 但这帧最后一次调用OnUpdate 时， 还是找到了
        //把这个代码块注释掉， 下面的组件 第一次就能找到
        if (!SystemAPI.TryGetSingletonEntity<PokerSystemInitFinishCData>(out Entity mTempEntity))
        {
            return;
        }


        int nCount = 0;
        Entity mTarget = Entity.Null;
        Entities.ForEach((ref PokerPoolCData PokerPoolObjRef) =>
        {
            mTarget = PokerPoolObjRef.Prefab;
            nCount++;
        }).Run();

        if(mTarget == Entity.Null) //这里检查 查询/实体 加载就绪状态
        {
            return;
        }

        Debug.Log($"找到 PokerPoolCData 的 Entity 数量: {nCount}");
        Unity.Assertions.Assert.IsTrue(mTarget != Entity.Null, "mTarget == null");

        EntityManager.DestroyEntity(mTempEntity);
        Enabled = false;
    }

3: 结构性变更: 触发条件: 增删任何Entity, 增删任何 IComponentData.
如果发生 结构性变更（Structural Change）会导致你:
[1] 缓存的任何IComponentData全部失效！无论你访问的是哪个组件、哪个 Entity！
[2] 本帧的查询 不及时，得ResetFilter() 或者下一帧 查询。 比如出现 删除Entity 没销毁干净的问题

4: Entity 本身是一个“稳定句柄”（stable handle），即使发生结构性变更（如添加/删除组件、原型变更），Entity 的值（Index + Version）仍然有效，不会“失效”或“悬空”

5: Hybrid方式：“混合模式”。
即实体（Entity）只负责位置、动画等逻辑, 渲染交给 GameObject 系统. 通过 UnityObjectRef<GameObject> 字段, 来与GameObject 世界中的物体进行交互。虽然不是纯 ECS，但能利用 ECS 的性能优势。
空当接龙中， 如何获取Entity的UI渲染组件或SpriteRenderer组件，（现在官方还没有）。 要么通过MeshRenderer 写一套类似 SpriteRenderer的组件，要么就是用Hybrid混合模式。

6: EntityCommandBuffer ecb2 = new EntityCommandBuffer(Allocator.Temp)：
必须手动 ecb2.Playback(EntityManager); 否则不执行
EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);
自动执行，自动释放。

7: LocalToWorld.Position	世界坐标（World Space）	从局部 → 世界变换后的最终位置
LocalTransform.Position	局部坐标（Local Space）	相对于父节点的位置（若无父节点，则等同世界坐标）

8: Data 如果包含 NativeList 等相关集合，不能用于GameObject烘培, 但能用于单例。