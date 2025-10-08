using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public struct PokerRenderCData : IComponentData
{
    //public Sprite mSprite;
    //public Vector2 Size;
}

[MaterialProperty("_MainTex")]
public struct Material_Color_CData : IComponentData
{
    public float4 Value;
}

[MaterialProperty("_MainTex")]
public struct Material_MainTex_CData : IComponentData
{
    public UnityObjectRef<Texture2D> Value;
}

[MaterialProperty("_MainTex_ST")]
public struct Material_MainTex_ST_CData : IComponentData
{
    public float4 Value;
}


public class ECS2SpriteRenderer
{
    //public Sprite mSprite;
    //public Vector2 Size;
    //public Material mMaterial;

    public void CreateMesh(EntityManager mEntityManager, Entity mEntity)
    {
        //var filterSettings = RenderFilterSettings.Default;
        //filterSettings.ShadowCastingMode = ShadowCastingMode.Off;
        //filterSettings.ReceiveShadows = false;

        //List<Vector2> uvs = new List<Vector2>();
        //uvs.Add(new Vector2(0, 0));
        //uvs.Add(new Vector2(0, 1));
        //uvs.Add(new Vector2(1, 1));
        //uvs.Add(new Vector2(1, 0));

        //List<int> triangles = new List<int>();
        //triangles.Add(0);
        //triangles.Add(1);
        //triangles.Add(2);
        //triangles.Add(2);
        //triangles.Add(3);
        //triangles.Add(0);

        //List<Vector3> vertices = new List<Vector3>();
        //vertices.Add(new Vector2(-Size.x / 2, -Size.x / 2));
        //vertices.Add(new Vector2(-Size.x / 2, Size.x / 2));
        //vertices.Add(new Vector2(Size.x / 2, Size.x / 2));
        //vertices.Add(new Vector2(Size.x / 2, 0));

        //Mesh mMesh = new Mesh();
        //mMesh.uv = uvs.ToArray();
        //mMesh.triangles = triangles.ToArray();
        //mMesh.vertices = vertices.ToArray();

        ////Material mMat = new Material(Shader.Find(""));


        //RenderMeshArray renderMeshArray = new RenderMeshArray(new Material[] {  }, new Mesh[] { mMesh });
        //var renderMeshDescription = new RenderMeshDescription
        //{
        //    FilterSettings = filterSettings,
        //    LightProbeUsage = LightProbeUsage.Off,
        //};

        //RenderMeshUtility.AddComponents(
        //    mEntity,
        //    mEntityManager,
        //    renderMeshDescription,
        //    renderMeshArray,
        //    MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

        //mEntityManager.AddComponentData(mEntity, new MaterialColor());
        //mEntityManager.AddComponentData(mEntity, new RenderBounds() { Value = mMesh.bounds.ToAABB() });
    }

    //public void UpdateMesh(EntityManager mEntityManager)
    //{
    //    //EntityCommandBuffer ecbJob = new EntityCommandBuffer(Allocator.TempJob);

    //    var filterSettings = RenderFilterSettings.Default;
    //    filterSettings.ShadowCastingMode = ShadowCastingMode.Off;
    //    filterSettings.ReceiveShadows = false;

    //    List<Vector2> uvs = new List<Vector2>();
    //    uvs.Add(new Vector2(0, 0));
    //    uvs.Add(new Vector2(0, 1));
    //    uvs.Add(new Vector2(1, 1));
    //    uvs.Add(new Vector2(1, 0));

    //    List<int> triangles = new List<int>();
    //    triangles.Add(0);
    //    triangles.Add(1);
    //    triangles.Add(2);
    //    triangles.Add(2);
    //    triangles.Add(3);
    //    triangles.Add(0);

    //    List<Vector3> vertices = new List<Vector3>();
    //    vertices.Add(new Vector2(0, 0));
    //    vertices.Add(new Vector2(0, 1));
    //    vertices.Add(new Vector2(1, 1));
    //    vertices.Add(new Vector2(1, 0));

    //    Mesh mMesh = new Mesh();
    //    mMesh.uv = uvs.ToArray();
    //    mMesh.triangles = triangles.ToArray();
    //    mMesh.vertices = vertices.ToArray();

    //    RenderMeshArray renderMeshArray = new RenderMeshArray(new Material[] { }, new Mesh[] { mMesh });
    //    var renderMeshDescription = new RenderMeshDescription
    //    {
    //        FilterSettings = filterSettings,
    //        LightProbeUsage = LightProbeUsage.Off,
    //    };

    //    var mEntity = mEntityManager.CreateEntity();
    //    RenderMeshUtility.AddComponents(
    //        mEntity,
    //        mEntityManager,
    //        renderMeshDescription,
    //        renderMeshArray,
    //        MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

    //    mEntityManager.AddComponentData(mEntity, new MaterialColor());
    //    mEntityManager.AddComponentData(mEntity, new RenderBounds() { Value = mMesh.bounds.ToAABB() });
    //}
}