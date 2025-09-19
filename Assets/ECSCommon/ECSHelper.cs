using Unity.Entities;
using UnityEngine;

public static class ECSHelper
{
    public static Entity FindChildEntityByName(EntityManager em, Entity rootEntity, string targetName)
    {
        if (!em.HasBuffer<LinkedEntityGroup>(rootEntity))
        {
            Debug.LogWarning("ʵ��û�� LinkedEntityGroup�����ܲ��Ǵ� Prefab ʵ������δ��ȷת����");
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

        Debug.LogWarning($"δ�ҵ�����Ϊ \"{targetName}\" ����ʵ�塣");
        return Entity.Null;
    }
}
