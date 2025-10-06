using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_SampleGI")]
    struct SampleGIFloatOverride : IComponentData
    {
        public float Value;
    }
}
