using UnityEngine;
using UnityEditor;
using System.IO;

public static class CreateMeshInEditor
{
    [MenuItem("Tools/Create Quad Mesh")]
    static void CreateQuadMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "QuadMesh";

        float nWidth = 103 / 100f;
        float nHigh = 154 / 100f;

        // 顶点
        var vertices = new Vector3[]
        {
            new Vector3(-nWidth / 2, -nHigh / 2, 0),
            new Vector3(-nWidth / 2, nHigh / 2, 0),
            new Vector3(nWidth / 2, nHigh / 2, 0),
            new Vector3(nWidth / 2, -nHigh / 2, 0)
        };

        // UV
        var uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };

        // 三角形索引
        var triangles = new[] { 0, 1, 2, 2, 3, 0 };

        // 设置数据
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        mesh.UploadMeshData(true); // 优化：上传后释放系统内存

        // 保存为 Asset
        string path = "Assets/Models/SpriteRendererMesh.asset";
        CreateDirectoryForPath(path);

        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Mesh created at: " + path);
    }

    static void CreateDirectoryForPath(string path)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}