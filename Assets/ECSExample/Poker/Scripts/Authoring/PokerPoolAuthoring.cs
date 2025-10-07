using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

//∆ÀøÀ≈∆ Image ∫Ê≈‡∆˜
public class PokerPoolAuthoring : MonoBehaviour
{
    public GameObject goPrefab;
    public int nPoolCount = 100;
    public List<Material> m_MaterialList;
    public List<Mesh> m_MeshList;

    class mBaker : Baker<PokerPoolAuthoring>
    {
        public override void Bake(PokerPoolAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PokerPoolCData
            {
                Prefab = GetEntity(authoring.goPrefab, TransformUsageFlags.Renderable | TransformUsageFlags.WorldSpace),
                Count = authoring.nPoolCount,
            });

            Debug.Log("PokerPoolAuthoring: ∫Ê≈‡");
        }
    }
}

public struct PokerPoolCData: IComponentData
{
    public Entity Prefab;
    public int Count;

}

