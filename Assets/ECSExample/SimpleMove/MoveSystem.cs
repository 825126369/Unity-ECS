using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[RequireMatchingQueriesForUpdate]
[BurstCompile]
public partial class SimpleMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        Entities.ForEach((ref LocalTransform transform, ref MoveParam moveSpeed, ref SimpleMoveObj obj) =>
        {
            if (transform.Position.x > 5)
            {
                moveSpeed.x = -4;
            }
            else if (transform.Position.x < -5)
            {
                moveSpeed.x = 4;
            }

            // Debug.Log("移动： " + speed); //使用Debug 会导致 报错：编辑器内存泄露错误
            transform.Position += new float3(moveSpeed.x, 0, 0) * deltaTime; // 每秒向右移动 1 单位

            // 或者直接设置一个新位置
            // transform.Position = new float3(0, 0, 0);
            // 也可以同时修改旋转和缩放
            // transform.Rotation = quaternion.RotateY(transform.Rotation.value, 0.1f * deltaTime);
            // transform.Scale = 1.5f;

        }).ScheduleParallel(); // 使用 Job System 并行执行

        ////查询组件，更新位置
        //foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>())
        //{
        //    Debug.Log("移动");
        //    transform.ValueRW.Position += new float3(0.5f, 0, 0) * deltaTime;
        //}
    }
}
