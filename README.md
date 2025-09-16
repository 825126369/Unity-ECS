# Unity-ECS-教程

世界是我的，也是你的，终究是属于年轻人的。 长江后浪推前浪，前浪死在沙滩上，江山代有才人出，一代新颜换旧颜。

UI Toolkit: 
(1) 得加上 InputSystemUIInputModule 这个组件，否则 Button 点击无响应
(2) VisualElement Display选项，得设置Display打开，否则在UI Builder中，无法可视化预览效果。
(3) PanelSettings 中 Clear Color 勾选得去掉，否则会黑屏

DOTS 的基础设施：
1： URP, UI Toolkit, New Input System, ECS 得务必首先把这些 组件加到项目中来。

2：SubScene 
(1) SubScene 是 Authoring(创作) 的容器/舞台/编辑器，顾名思义: SubScene 就是 在GameObject世界里进行创作，通过在编译阶段烘培为 Entity世界。游戏启动后，创作阶段的GameObject世界全都会被Skip掉. 所以 SubScene 下面的所有GameObject 在运行阶段都不会执行 Awake, Start 这些生命周期函数，因为 这些GameObject 都被忽略掉了。
(2) SubScene 需要点击 New Sub Scene 创建，手动添加 SubScene 会有问题。

如果你看到物体没渲染出来， 不管是编辑模式，还是运行模式，都是前两步出错导致