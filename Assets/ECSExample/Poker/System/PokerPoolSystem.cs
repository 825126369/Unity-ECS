using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Boids
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct PokerPoolSystem : ISystem
    {
        private EntityQuery _PokerQuery;
        public void OnCreate(ref SystemState state)
        {
            _PokerQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PokerAnimationCData, LocalTransform>().Build(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var world = state.World.Unmanaged;

            foreach (var (PokerPoolObjRef, LocalToWorldRef, entity) in SystemAPI.Query<RefRO<PokerPoolCData>, RefRO<LocalToWorld>>().WithEntityAccess())
            {
                NativeArray<Entity> boidEntities = CollectionHelper.
                    CreateNativeArray<Entity, RewindableAllocator>(PokerPoolObjRef.ValueRO.Count, ref world.UpdateAllocator);
                state.EntityManager.Instantiate(PokerPoolObjRef.ValueRO.Prefab, boidEntities);

                var LocalToWorldJob = new SetPokerLocalToWorld
                {
                    LocalToWorldFromEntity = localToWorldLookup,
                    Entities = boidEntities,
                    Center = LocalToWorldRef.ValueRO.Position,
                    Radius = 100
                };

                state.Dependency = LocalToWorldJob.Schedule(PokerPoolObjRef.ValueRO.Count, 64, state.Dependency);
                state.Dependency.Complete();
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);

            //这里必须去掉LocalTransform,否则设置 LocalToWorld 无效，因为 LocalTransform 会更新LocalToWorld 组件
            state.EntityManager.RemoveComponent<LocalTransform>(_PokerQuery);
        }

    }

    [BurstCompile]
    struct SetPokerLocalToWorld : IJobParallelFor
    {
        [NativeDisableContainerSafetyRestriction]
        [NativeDisableParallelForRestriction] //// “危险解除”属性
        public ComponentLookup<LocalToWorld> LocalToWorldFromEntity;

        public NativeArray<Entity> Entities;
        public float3 Center;
        public float Radius;

        public void Execute(int i)
        {
            var entity = Entities[i];
            var random = new Random(((uint)(entity.Index + i + 1) * 0x9F6ABC1));
            var dir = math.normalizesafe(random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f));
            var pos = Center + (dir * Radius);

            var localToWorld = new LocalToWorld
            {
                Value = float4x4.TRS(pos, quaternion.LookRotationSafe(dir, math.up()), new float3(1f, 1f, 1f))
            };
            LocalToWorldFromEntity[entity] = localToWorld; ////给这个实体 赋值 LocalToWorld 本地到世界矩阵变换
            //这个赋值之后
        }
    }
}
