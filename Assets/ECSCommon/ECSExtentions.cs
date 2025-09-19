using Unity.Mathematics;
using Unity.Transforms;

public static class ECSExtentions
{
    //将世界坐标转换为相对于给定 LocalToWorld 的局部坐标
    public static float3 WorldToLocal(this LocalToWorld parentLtw, float3 worldPosition)
    {
        return math.transform(math.inverse(parentLtw.Value), worldPosition);
    }

    //将局部坐标转换为相对于给定 LocalToWorld 的世界坐标
    public static float3 LocalToWorld(this LocalToWorld parentLtw, float3 localPosition)
    {
        return math.transform(parentLtw.Value, localPosition);
    }
}
