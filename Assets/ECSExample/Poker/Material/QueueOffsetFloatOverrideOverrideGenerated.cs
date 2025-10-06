using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_QueueOffset")]
    struct QueueOffsetFloatOverride : IComponentData
    {
        public float Value;
    }
}
