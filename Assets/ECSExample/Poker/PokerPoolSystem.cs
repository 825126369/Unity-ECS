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
        private EntityQuery _objQuery;
        private static readonly int nMaxCount = 10000;

        public void OnCreate(ref SystemState state)
        {
            _objQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PokerObj, LocalTransform>().Build(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var world = state.World.Unmanaged;

            foreach(var (PokerObjRef, LocalToWorldRef, entity) in SystemAPI.Query<RefRO<PokerObj>, RefRO<LocalToWorld>>().WithEntityAccess())
            {
                var boidEntities = CollectionHelper.CreateNativeArray<Entity, RewindableAllocator>(nMaxCount, ref world.UpdateAllocator);
                state.EntityManager.Instantiate(PokerObjRef.ValueRO.Prefab, boidEntities);
                var LocalToWorldJob = new SetPokerLocalToWorld
                {
                    LocalToWorldFromEntity = localToWorldLookup,
                    Entities = boidEntities,
                    Center = LocalToWorldRef.ValueRO.Position,
                };
                state.Dependency = LocalToWorldJob.Schedule(nMaxCount, 64, state.Dependency);
                state.Dependency.Complete();
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            state.EntityManager.RemoveComponent<LocalTransform>(_objQuery);
        }

    }

    [BurstCompile]
    struct SetPokerLocalToWorld : IJobParallelFor
    {
        [NativeDisableContainerSafetyRestriction] [NativeDisableParallelForRestriction]
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
                Value = float4x4.TRS(pos, quaternion.LookRotationSafe(dir, math.up()), new float3(1.0f, 1.0f, 1.0f))
            };
            LocalToWorldFromEntity[entity] = localToWorld;
        }
    }
}
