using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDuck : MonoBehaviour
{
    public float headHeight;
    public LayerMask terrainLayer;
    
    public float maxVelocity = 5.0f;
    public float minVelocity = 1.0f;
    public float behindVelocityMultiplier = 3.0f;

    public float accelerationTime = 2.0f;

    public float aheadMinRange = 1.0f;
    public float midRange;
    public float panicRange;

    public enum MiniDuckState
    {
        Idle,
        Ahead,
        Mid,
        Behind,
    }

    [SerializeField, NaughtyAttributes.ReadOnly]
    MiniDuckState state;
    
    float velocity;
    float targetVelocity;
    float refVel;

    void Awake ()
    {
        velocity = minVelocity;
        state = MiniDuckState.Idle;
    }

    public void UpdatePosition (Vector3 targetPosition)
    {
        float zdist = (transform.position.z - targetPosition.z);

        if(zdist > aheadMinRange)
        {
            state = MiniDuckState.Ahead;
            targetVelocity = minVelocity;
        }
        else if (zdist > -midRange)
        {
            if(state == MiniDuckState.Behind || state == MiniDuckState.Ahead)
                targetVelocity = Random.Range(minVelocity, maxVelocity);

            state = MiniDuckState.Mid;
        }
        else if (zdist < -(midRange + panicRange))
        {
            state = MiniDuckState.Behind;
            targetVelocity = maxVelocity * behindVelocityMultiplier;
        }
        else
        {
            if(state == MiniDuckState.Behind || state == MiniDuckState.Ahead)
                targetVelocity = Random.Range(minVelocity, maxVelocity);

            state = MiniDuckState.Mid;
        }

        velocity = Mathf.SmoothDamp(velocity, targetVelocity, ref refVel, accelerationTime);

        Vector3 next = transform.position + Vector3.forward * velocity * Time.deltaTime;
        next.y = headHeight;

        if(Physics.Raycast(transform.position + Vector3.up * 5.0f, Vector3.down, out RaycastHit hit, 10.0f, terrainLayer.value))
            next.y += hit.point.y;
        
        transform.position = next;
    }
}
