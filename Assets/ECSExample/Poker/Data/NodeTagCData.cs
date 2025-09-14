using Unity.Collections;
using Unity.Entities;

public struct NodeTagCData : IComponentData
{
    public FixedString32Bytes Value;
}