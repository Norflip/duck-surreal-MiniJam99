using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct MeshHeightJob : IJobParallelFor
{
    public NativeArray<Vector3> vertices;

    public float scale; // 1
    public float height; // 3
    public float positionZOffset;

    public void Execute(int index)
    {
        Vector3 vert = vertices[index];
        float2 p = math.float2(vert.x * scale, (vert.z + positionZOffset) * scale);
        vertices[index] = new Vector3(vert.x, noise.snoise(p), vert.z);
    }
}

public class Chunk : MonoBehaviour
{
    public const int Size = 32;

    public int Index => index;
    [SerializeField] int index;
    Mesh mesh;
    MeshCollider coll;

    private void OnValidate() {
        transform.position = new Vector3(0.0f, 0.0f, index * Size);
    }

    public void Generate (int index, float noiseScale, float height)
    {
        if(mesh == null)
        {
            mesh = GetComponent<MeshFilter>().mesh; 
            coll = GetComponent<MeshCollider>();
        }
    
        NativeArray<Vector3> vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob); // 2

        this.index = index;
        transform.position = new Vector3(0.0f, 0.0f, index * Size);

        MeshHeightJob job = new MeshHeightJob();
        job.scale = noiseScale;
        job.height = height;
        job.vertices = vertices;
        job.positionZOffset = index * Size;
        job.Schedule(vertices.Length, 64).Complete();

        mesh.SetVertices(job.vertices);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        vertices.Dispose();

        if(coll != null)
        {
            coll.sharedMesh = null;
            coll.sharedMesh = mesh;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0.0f, 0.0f, index * Size), new Vector3(Size, 0, Size));
    }
}
