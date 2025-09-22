using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public interface KKIComponentData : IComponentData
{
    public void Init(GameObject gameObject);
}

public struct GameObjectCData : KKIComponentData
{
    public FixedString32Bytes Name;

    public void Init(GameObject go)
    {
        this.Name = go.name;
    }

    public static GameObjectCData GetData(GameObject gameObject)
    {
        GameObjectCData mData = new GameObjectCData();
        mData.Init(gameObject);
        return mData;
    }
}

public struct SpriteRendererCData : KKIComponentData
{
    public int nOrderId;
    public FixedString32Bytes atlasName;
    public FixedString32Bytes spriteName;

    public void Init(GameObject gameObject)
    {
        SpriteRenderer m = gameObject.GetComponent<SpriteRenderer>();
    }
}

public struct TempSpriteRendererUpdateCData : IComponentData
{
    public int nOrderId;
    public FixedString32Bytes atlasName;
    public FixedString32Bytes spriteName;
}