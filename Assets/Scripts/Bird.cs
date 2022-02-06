using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour, IPoolable<Bird>
{
    [NaughtyAttributes.Layer]
    public int paintingLayer;
    public float timeToPaint = 0.2f;
    [NaughtyAttributes.Layer]
    public int layerAfterPainted;

    public ParticleSystem collisionParticles;
    public AudioClip collisionClip;
    
    public float collisionNormalForce = 500.0f;
    
    public int hits = 0;

    float gottenTime;

    Rigidbody body;
    Renderer rend;
    Pool<Bird> pool;

    private void Awake() {
        body = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();

        //rend.
    }

    private void Update() {
        if(gottenTime + 2.0f < Time.time || (rend != null && !rend.isVisible))
        {
            pool.Return(this);
        }    
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == paintingLayer)
        {
            Painting painting = other.gameObject.GetComponent<Painting>();
            painting.TriggerBrush();
            painting.Shake();

            body.AddForce(painting.GetPlane().normal * collisionNormalForce);

            StartCoroutine(SetLayer());
            hits++;
        }
    }

    public void Added(Pool<Bird> pool) {
        this.pool = pool;
    }

    public void Gotten() {
        hits = 0;
        gottenTime = Time.time;
        body.velocity = body.angularVelocity = Vector3.zero;
    }

    public void Returned() 
    {

    }

    IEnumerator SetLayer ()
    {
        //yield return new WaitForSeconds(timeToPaint);
        yield return new WaitForEndOfFrame();

        gameObject.layer = layerAfterPainted;

    }
    
}
