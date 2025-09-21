using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public static class ECSHelper
{
    public static void AddMissComponentData<T>(EntityManager em, Entity mEntity)
        where T: unmanaged, IComponentData
    {
        if(!em.HasComponent<T>(mEntity))
        {
            em.AddComponent<T>(mEntity);
        }
    }

    public static Entity FindChildEntityByName(EntityManager em, Entity rootEntity, string targetName)
    {
        if (!em.HasBuffer<LinkedEntityGroup>(rootEntity))
        {
            Debug.LogWarning("实体没有 LinkedEntityGroup，可能不是从 Prefab 实例化或未正确转换。");
            return Entity.Null;
        }


        var buffer = em.GetBuffer<LinkedEntityGroup>(rootEntity);
        foreach (var linkedEntity in buffer)
        {
            Entity child = linkedEntity.Value;
            if (em.HasComponent<GameObjectCData>(child) && em.GetComponentData<GameObjectCData>(child).Name == targetName)
            {
                return child;
            }
        }

        Debug.LogWarning($"未找到名字为 \"{targetName}\" 的子实体。");
        return Entity.Null;
    }

    public static void Enable(EntityManager em, Entity rootEntity, bool bEnable)
    {
        if (!em.HasBuffer<LinkedEntityGroup>(rootEntity))
        {
            Debug.LogWarning("实体没有 LinkedEntityGroup，可能不是从 Prefab 实例化或未正确转换。");
            return;
        }

        var buffer = em.GetBuffer<LinkedEntityGroup>(rootEntity);
        NativeList<Entity> mEntityList = new NativeList<Entity>(Allocator.Temp);
        foreach (var linkedEntity in buffer)
        {
            Entity child = linkedEntity.Value;
            mEntityList.Add(child);
        }

        foreach(var v in mEntityList)
        {     
            if (bEnable)
            {
                if (em.HasComponent<Disabled>(v))
                {
                    em.RemoveComponent<Disabled>(v);
                }
            }
            else
            {
                if (!em.HasComponent<Disabled>(v))
                {
                    em.AddComponent<Disabled>(v);
                }
            }
        }
    }
}
