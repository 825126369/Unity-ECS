//using Unity.Burst;
//using Unity.Collections;
//using Unity.Collections.LowLevel.Unsafe;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Transforms;

//[RequireMatchingQueriesForUpdate]
//[BurstCompile]
//public partial struct PokerPoolSystem : ISystem
//{
//    private EntityQuery _PokerQuery;
//    public void OnCreate(ref SystemState state)
//    {
//        _PokerQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PokerAnimationCData, LocalTransform>().Build(ref state);
//    }

//    public void OnUpdate(ref SystemState state)
//    {
//        ComponentLookup<LocalTransform> LocalTransformFromEntity = SystemAPI.GetComponentLookup<LocalTransform>();
//        var ecb = new EntityCommandBuffer(Allocator.Temp);
//        var world = state.World.Unmanaged;

//        foreach (var (PokerPoolObjRef, LocalToWorldRef, entity) in
//            SystemAPI.Query<RefRO<PokerPoolCData>, RefRO<LocalToWorld>>().WithEntityAccess())
//        {
//            NativeArray<Entity> boidEntities = CollectionHelper.
//                CreateNativeArray<Entity, RewindableAllocator>(PokerPoolObjRef.ValueRO.Count, ref world.UpdateAllocator);
//            state.EntityManager.Instantiate(PokerPoolObjRef.ValueRO.Prefab, boidEntities);

//            var LocalToWorldJob = new SetPokerLocalToWorldJob
//            {
//                LocalTransformFromEntity = LocalTransformFromEntity,
//                Entities = boidEntities,
//                Center = LocalToWorldRef.ValueRO.Position,
//                Radius = 10
//            };

//            state.Dependency = LocalToWorldJob.Schedule(PokerPoolObjRef.ValueRO.Count, 64, state.Dependency);
//            state.Dependency.Complete();
//            ecb.DestroyEntity(entity);
//        }

//        ecb.Playback(state.EntityManager);
//        //这里必须去掉LocalTransform,否则设置 LocalToWorld 无效，因为 LocalTransform 会更新LocalToWorld 组件
//        //state.EntityManager.RemoveComponent<LocalTransform>(_PokerQuery);
//    }
//}

//[BurstCompile]
//struct SetPokerLocalToWorldJob : IJobParallelFor
//{
//    [NativeDisableContainerSafetyRestriction]
//    [NativeDisableParallelForRestriction] //// “危险解除”属性
//    public ComponentLookup<LocalTransform> LocalTransformFromEntity;
//    public NativeArray<Entity> Entities;
//    public float3 Center;
//    public float Radius;

//    public void Execute(int i)
//    {
//        var entity = Entities[i];

//        var random = new Random(((uint)(entity.Index + i + 1) * 0x9F6ABC1));
//        var dir = math.normalizesafe(random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f));
//        var pos = Center + (dir * 5);

//        //pos = Center + new float3(i * 1, 0, 0);
//        quaternion qua = quaternion.identity;
//        float scale = 1;

//        LocalTransform mLocalTransform = new LocalTransform();
//        mLocalTransform.Position = pos;
//        mLocalTransform.Rotation = qua;
//        mLocalTransform.Scale = scale;
//        LocalTransformFromEntity[entity] = mLocalTransform;
//    }
//}

////这种直接设置 LocalToWorld 对 [SpriteRenderer] 不起作用，初步排查，应该是 虽然变化矩阵更新了，但 SpriteRenderer 没去渲染，
////这样位置永远是 0 点了
//[BurstCompile]
//struct SetPokerLocalToWorldJob2 : IJobParallelFor
//{
//    [NativeDisableContainerSafetyRestriction]
//    [NativeDisableParallelForRestriction] //// “危险解除”属性
//    public ComponentLookup<LocalToWorld> LocalToWorldFromEntity;

//    public NativeArray<Entity> Entities;
//    public float3 Center;
//    public float Radius;

//    public void Execute(int i)
//    {
//        var entity = Entities[i];
//        var random = new Random(((uint)(entity.Index + i + 1) * 0x9F6ABC1));
//        var dir = math.normalizesafe(random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f));
//        var pos = Center + (dir * 10);

//        pos = Center + new float3(i * 1, 0, 0);
//        quaternion qua = quaternion.identity;
//        float3 scale = 1;
//        var localToWorld = new LocalToWorld
//        {
//            Value = float4x4.TRS(pos, qua, scale)
//        };

//        LocalToWorldFromEntity[entity] = localToWorld; ////给这个实体 赋值 LocalToWorld 本地到世界矩阵变换

//        //这个方法的作用，就是我直接计算 Shader 中的变换矩阵，把LocalTransform去掉，不走TransformUpdateGroup了
//        //这种直接设置 LocalToWorld 对 SpriteRenderer 不起作用，初步排查，应该是 虽然变化矩阵更新了，但 SpriteRenderer 没去渲染，
//        //这样位置永远是 0 点了
//    }

//}
