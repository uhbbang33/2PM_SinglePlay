using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;   // StopWatch
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    Mesh terrainMesh;

    public Vector3[] vertices;
    private int[] triangles;

    [SerializeField]
    private float mapHeight;
    [SerializeField]
    private int verticesSize;       // 지형의 한 면의 vertices 개수(이름 다시 정하기)
    [SerializeField]
    private float perlinNoiseHeight;    // perlinNoise의 정도
    [SerializeField]
    private int distanceOfMeshToMesh;

    // Awake로 사용할지 Start로 사용할지 고민하기
    void Awake()
    {
        terrainMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = terrainMesh;
        vertices = new Vector3[(verticesSize + 1) * (verticesSize + 1)];    //10 * 10 = 100
    }

    void Start()
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();

        // Vectices Position
        int i = 0;
        for (int z = 0; z <= verticesSize; ++z)
            for (int x = 0; x <= verticesSize; ++x)
            {
                // Terrain Height - use PerlinNoise
                vertices[i] = new Vector3(x * distanceOfMeshToMesh,
                                Mathf.PerlinNoise(x * perlinNoiseHeight, z * perlinNoiseHeight) * mapHeight,
                                z * distanceOfMeshToMesh);
                ++i;
            }

        // CreateTriangle
        int v = 0, t = -1;
        triangles = new int[verticesSize * verticesSize * 6];

        for (int j = 0; j < verticesSize * verticesSize; ++j)
        {
            triangles[++t] = v;
            triangles[++t] = v + verticesSize + 1;
            triangles[++t] = v + 1;
            triangles[++t] = v + 1;
            triangles[++t] = v + verticesSize + 1;
            triangles[++t] = v + verticesSize + 2;

            ++v;
        }
       
        UpdateMesh();

        watch.Stop();
        UnityEngine.Debug.Log(watch.ElapsedMilliseconds + " ms");
    }

    public void UpdateMesh()
    {
        terrainMesh.Clear();
        terrainMesh.vertices = vertices;
        terrainMesh.triangles = triangles;

        terrainMesh.RecalculateNormals();

        MeshCollider Meshcol = GetComponent<MeshCollider>();
        Meshcol.sharedMesh = terrainMesh;
    }
}
