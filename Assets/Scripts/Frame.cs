using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frame : MonoBehaviour
{
    public float thickness = 0.2f;
    public float depth = 0.08f;
    public Transform scaleReference;
    
    [NaughtyAttributes.Button("mesh")]
    public void Generate ()
    {
        Vector3 localp = transform.localPosition;
        localp.y = scaleReference.localPosition.y;
        transform.localPosition = localp;

        Vector2 size = scaleReference.localScale / 2.0f;
        
        const int pointInFrameSegment = 3;
        
        Vector3[] vertices = new Vector3[4 * pointInFrameSegment];
        int[] triangles = new int[4 * 2 * 6];
        int ti = 0;

        Vector2[] dirs = new Vector2[] { new Vector2(1,1),new Vector2(1,-1),new Vector2(-1,-1),new Vector2(-1,1) };

        for (int j = 0; j < 4; j++)
        {
            Vector2 offset = new Vector2(dirs[j].x * size.x, dirs[j].y * size.y);
                
            for (int x = 0; x < pointInFrameSegment; x++)
            {
                int vindex = j * pointInFrameSegment + x;
                int nextVIndex = ((j+1) * pointInFrameSegment + x) % (12);
                
                float t = ((float)(thickness + 1) / (float)pointInFrameSegment) * thickness;
                float px = offset.x + dirs[j].x * x * t;
                float py = offset.y + dirs[j].y * x * t;
                
                Vector3 p = new Vector3(px, py, (x > 0 && x < pointInFrameSegment - 1)? depth : 0);
                vertices[vindex] = p;

                if(j < 4 && x < pointInFrameSegment -1)
                {
                    triangles[ti + 0] = vindex;
                    triangles[ti + 1] = vindex + 1;
                    triangles[ti + 2] = (vindex + pointInFrameSegment + 1) % vertices.Length;

                    triangles[ti + 3] = vindex;
                    triangles[ti + 4] = (vindex + pointInFrameSegment + 1) % vertices.Length;
                    triangles[ti + 5] = (vindex + pointInFrameSegment) % vertices.Length;
        
                    ti += 6;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "frame";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider collider = GetComponent<MeshCollider>();
        if(collider != null)
        {
            collider.sharedMesh = null;
            collider.sharedMesh = mesh;
        }
    }
}
