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
    public SoundCollection collisionSound;
    
    public float collisionNormalForce = 500.0f;
    public int hits = 0;

    float gottenTime;
    bool isDespawning = false;

    [HideInInspector]
    public bool launched = false;

    [HideInInspector]
    public Rigidbody body;
    Renderer rend;
    Pool<Bird> pool;

    private void Awake() {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        rend = GetComponent<Renderer>();

        //rend.
    }

    private void Update() {
        if(!launched)
            return;

        if(!isDespawning && (gottenTime + 2.0f < Time.time || (rend != null && !rend.isVisible)))
        {
            StartCoroutine(Despawn());
        }    
    }

    IEnumerator Despawn ()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + Vector3.down * 2.0f;

        isDespawning = true;
        float t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        pool.Return(this);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == paintingLayer)
        {
            Painting painting = other.gameObject.GetComponent<Painting>();
            painting.TriggerBrush();
            painting.Shake();

            body.AddForce(painting.GetPlane().normal * collisionNormalForce);
            Particles.SpawnFeathers(painting.transform, transform.position);

            AudioRunner.Play3D(collisionSound, transform.position);

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
        isDespawning = false;
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
