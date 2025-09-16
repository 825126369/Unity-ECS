using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public static class PoolTagConst
{
    public const int Poker = 0;
}

public struct PoolTag : IComponentData
{
    public int PoolId; // 或使用 Hash128、FixedString 等
}

public partial class EntityPoolManager : SystemBase
{
    private static EntityPoolManager _instance;
    public static EntityPoolManager Instance => _instance;
    private NativeHashMap<int, NativeList<Entity>> _pools;

    protected override void OnCreate()
    {
        _instance = this;
        _pools = new NativeHashMap<int, NativeList<Entity>>();
    }

    protected override void OnDestroy()
    {
        foreach (KVPair<int, NativeList<Entity>> pool in _pools)
        {
            pool.Value.Dispose();
        }
        _pools.Clear();
    }

    protected override void OnUpdate()
    {

    }

    public Entity Spawn(Entity prefab, int poolId)
    {
        if (!_pools.TryGetValue(poolId, out var pool))
        {
            pool = new NativeList<Entity>(Allocator.Persistent);
            _pools[poolId] = pool;
        }

        for (int i = 0; i < pool.Length; i++)
        {
            var entity = pool[i];
            if (EntityManager.HasComponent<Disabled>(entity))
            {
                EntityManager.RemoveComponent<Disabled>(entity);
                pool.RemoveAtSwapBack(i);
                ResetEntity(entity);
                return entity;
            }
        }

        var newEntity = EntityManager.Instantiate(prefab);
        EntityManager.AddComponentData(newEntity, new PoolTag { PoolId = poolId });
        return newEntity;
    }

    public void Recycle(Entity entity)
    {
        if (!EntityManager.Exists(entity) || !EntityManager.HasComponent<PoolTag>(entity))
            return;

        var poolTag = EntityManager.GetComponentData<PoolTag>(entity);
        int poolId = poolTag.PoolId;

        if (!_pools.TryGetValue(poolId, out var pool))
        {
            pool = new NativeList<Entity>(Allocator.Persistent);
            _pools[poolId] = pool;
        }

        ResetEntity(entity);
        pool.Add(entity);
    }

    private void ResetEntity(Entity entity)
    {
        if (!EntityManager.HasComponent<Disabled>(entity))
        {
            EntityManager.AddComponent<Disabled>(entity);
        }

        if (EntityManager.HasComponent<LocalTransform>(entity))
        {
            var lt = EntityManager.GetComponentData<LocalTransform>(entity);
            lt.Position = float3.zero;
            lt.Rotation = quaternion.identity;
            lt.Scale = 1.0f;
            EntityManager.SetComponentData(entity, lt);
        }

        if (EntityManager.HasComponent<PokerAnimationCData>(entity))
        {
            var lt = EntityManager.GetComponentData<PokerAnimationCData>(entity);
            lt.Reset();
        }

        //if (EntityManager.HasComponent<AttackComponent>(entity))
        //{
        //    EntityManager.RemoveComponent<AttackComponent>(entity);
        //}

        //if (EntityManager.HasComponent<PlayAudioRequest>(entity))
        //{
        //    EntityManager.RemoveComponent<PlayAudioRequest>(entity);
        //}
    }
}