using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public struct PokerRenderCData : IComponentData
{
    public Sprite mSprite;
    public Vector2 Size;
}

public static class PokerRenderHeler
{ 
    public static void CreateMesh(EntityManager mEntityManager, Entity mEntity)
    {
        var filterSettings = RenderFilterSettings.Default;
        filterSettings.ShadowCastingMode = ShadowCastingMode.Off;
        filterSettings.ReceiveShadows = false;

        List<Vector2> uvs = new List<Vector2>();
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        List<int> triangles = new List<int>();
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(0);

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector2(0, 0));
        vertices.Add(new Vector2(0, 1));
        vertices.Add(new Vector2(1, 1));
        vertices.Add(new Vector2(1, 0));


        Mesh mMesh = new Mesh();
        mMesh.uv = uvs.ToArray();
        mMesh.triangles = triangles.ToArray();
        mMesh.vertices = vertices.ToArray();

        RenderMeshArray renderMeshArray = new RenderMeshArray(new Material[] { }, new Mesh[] { mMesh });
        var renderMeshDescription = new RenderMeshDescription
        {
            FilterSettings = filterSettings,
            LightProbeUsage = LightProbeUsage.Off,
        };
        
        RenderMeshUtility.AddComponents(
            mEntity,
            mEntityManager,
            renderMeshDescription,
            renderMeshArray,
            MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

        mEntityManager.AddComponentData(mEntity, new MaterialColor());
        mEntityManager.AddComponentData(mEntity, new RenderBounds() { Value = mMesh.bounds.ToAABB() });
    }

    public static void UpdateMesh(EntityManager mEntityManager)
    {
        EntityCommandBuffer ecbJob = new EntityCommandBuffer(Allocator.TempJob);

        var filterSettings = RenderFilterSettings.Default;
        filterSettings.ShadowCastingMode = ShadowCastingMode.Off;
        filterSettings.ReceiveShadows = false;

        List<Vector2> uvs = new List<Vector2>();
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        List<int> triangles = new List<int>();
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(0);

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector2(0, 0));
        vertices.Add(new Vector2(0, 1));
        vertices.Add(new Vector2(1, 1));
        vertices.Add(new Vector2(1, 0));


        Mesh mMesh = new Mesh();
        mMesh.uv = uvs.ToArray();
        mMesh.triangles = triangles.ToArray();
        mMesh.vertices = vertices.ToArray();

        RenderMeshArray renderMeshArray = new RenderMeshArray(new Material[] { }, new Mesh[] { mMesh });
        var renderMeshDescription = new RenderMeshDescription
        {
            FilterSettings = filterSettings,
            LightProbeUsage = LightProbeUsage.Off,
        };

        var mEntity = mEntityManager.CreateEntity();
        RenderMeshUtility.AddComponents(
            mEntity,
            mEntityManager,
            renderMeshDescription,
            renderMeshArray,
            MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

        mEntityManager.AddComponentData(mEntity, new MaterialColor());
        mEntityManager.AddComponentData(mEntity, new RenderBounds() { Value = mMesh.bounds.ToAABB() });
    }
}