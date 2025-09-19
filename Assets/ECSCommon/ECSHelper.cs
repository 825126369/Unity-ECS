using Unity.Entities;
using UnityEngine;

public static class ECSHelper
{
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
}
