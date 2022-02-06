using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDuckController : MonoBehaviour
{
    public MiniDuck prefab;
    public float disallowedRadius = 4.0f;
    public int count;

    public float height;
    public float width;
    public Transform player;
    public Transform miniDuckContainer;
    public bool run = true;

    List<Vector3> points;
    List<MiniDuck> ducks;

    private void Start() {
        points = new List<Vector3>();
        ducks = new List<MiniDuck>();

        Vector3 left = player.position + Vector3.left * width;
        int halfCount = Mathf.RoundToInt(count * 0.5f);

        for (int i = 0; i < count; i++)
        {
            float xOffset = (width + width) / (count-1) * (float)i;
            float zOffset = Random.Range(-height, height);

            Vector3 pos = left + Vector3.right * xOffset + Vector3.forward * zOffset;
            if(Mathf.Abs(pos.x - player.position.x) > disallowedRadius)
            {
                points.Add(pos);
                MiniDuck duck = Instantiate(prefab);
                duck.transform.SetParent(miniDuckContainer);
                duck.transform.position = pos;
                duck.transform.rotation = Quaternion.identity;// Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                ducks.Add(duck);
            }   
        }
    }

    private void Update() {
        if(run)
        {
            for (int i = 0; i < ducks.Count; i++)
            {
                Vector3 position = player.position + points[i];
                ducks[i].UpdatePosition(position);
            }
        }
    }

    private void OnDrawGizmos() {
        
        if(player == null)
            return;

        Vector3 left = player.position + Vector3.left * width;

        Random.State state = Random.state;
        Random.InitState(0);

        int halfCount = Mathf.RoundToInt(count * 0.5f);

        for (int i = 0; i < count; i++)
        {
            float xOffset = (width + width) / (count-1) * (float)i;
            float zOffset = Random.Range(-height, height);

            Vector3 pos = left + Vector3.right * xOffset + Vector3.forward * zOffset;
            if(Mathf.Abs(pos.x - player.position.x) > disallowedRadius)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(pos, 0.2f);
            }
        }

        if(points != null)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.DrawSphere(points[i], 0.1f);
            }
        }
        
        if(player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(player.position - Vector3.left * width, player.position + Vector3.left * width);
            Gizmos.DrawLine(player.position - Vector3.left * width + Vector3.forward * height, player.position - Vector3.left * width - Vector3.forward * height);
        }

        Random.state = state;
    }
}
