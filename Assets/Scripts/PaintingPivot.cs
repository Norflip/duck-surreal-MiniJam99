using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingPivot : MonoBehaviour
{
    
    [Header("player and movement")]
    public Transform player;
    public float forwardDistace;
    public float hoverSpeed;
    public float hoverLength;
    public AnimationCurve hoverCurve;

    public float headHeight;
    public LayerMask terrainlayer;
    public bool billboard = true;
    [SerializeField, NaughtyAttributes.ReadOnly] float hover_t;

    void Update ()
    {
        UpdatePosition();
    }
    
    public void UpdatePosition ()
    {
        Vector3 next = player.position + Vector3.forward * forwardDistace;
        next.y = headHeight;

        if(Physics.Raycast(next + Vector3.up * 5.0f, Vector3.down, out RaycastHit hit, 10.0f, terrainlayer.value))
            next.y += hit.point.y;

        hover_t = hoverCurve.Evaluate(Mathf.PingPong(Time.time * hoverSpeed, 1.0f));
        next.y += hover_t * hoverLength;
   
        transform.position = next;

        if(billboard)
        {
            Vector3 toPlayer = (transform.position - player.position);
            toPlayer.y = 0.0f;
            toPlayer.Normalize();
            transform.localRotation = Quaternion.LookRotation(toPlayer, Vector3.up);
        }
    }
}
