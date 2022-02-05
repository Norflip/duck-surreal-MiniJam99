using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public float throwAngle = 30.0f;
    public Transform targetHitPoint;
    public Transform launchPivot;
    public LineRenderer projectilePath;

    public Painting painting;
    public bool allowInput;

    public Bird birdPrefab;

    private void Awake() {
        if(painting == null)
            painting = FindObjectOfType<Painting>();
        
        if(projectilePath == null)
            projectilePath = GetComponentInChildren<LineRenderer>();
    }

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(painting.PaintPlane.Raycast(ray, out float enter))
        {
            targetHitPoint.position = ray.GetPoint(enter);

            //if(Input.GetMouseButton(0))
            //    CalculateTrajectory ();

            if(Input.GetMouseButtonDown(0))
            {
                Vector3 launch = LaunchDirection() * LaunchForce();
                Bird bird = Instantiate(birdPrefab);
                Rigidbody body = bird.GetComponent<Rigidbody>();
                body.position = body.transform.position = launchPivot.position;
                body.AddForce(launch, ForceMode.VelocityChange);
                body.AddTorque(Random.onUnitSphere * 200.0f);
            }
        }
    }

    Vector3 LaunchDirection ()
    {
        float angle = throwAngle * Mathf.Deg2Rad;
        Quaternion yrot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);

        Vector3 dirxz = new Vector3(targetHitPoint.position.x - launchPivot.position.x, 0.0f, targetHitPoint.position.z - launchPivot.position.z);
        yrot = Quaternion.LookRotation(dirxz, Vector3.up);

        return (yrot * new Vector3(0.0f, Mathf.Sin(angle), Mathf.Cos(angle))).normalized;
    }

    float LaunchForce ()
    {
        float angle = throwAngle * Mathf.Deg2Rad;
        float distance = Vector3.Distance(targetHitPoint.position, transform.position);
        float yOffset = transform.position.y - targetHitPoint.position.y;

        float A = (1 / Mathf.Cos(angle));
        float B = (0.5f * -Physics.gravity.y * (distance * distance));
        float C = (distance * Mathf.Tan(angle) + yOffset);
        float D = B / C;
        float E = Mathf.Sqrt(D);

        return A * Mathf.Sqrt(B / C);
    }

    void CalculateTrajectory ()
    {
        float mass = 1.0f; // birdPrefab.GetComponent<Rigidbody>().mass;
        int steps = 32;
        projectilePath.positionCount = steps;
        Vector3[] points = new Vector3[steps];
        
        Vector3 position = launchPivot.position;
        Vector3 velocity = (LaunchDirection() * LaunchForce()) / mass;
        points[0] = position;

        for(int i = 1; i  < steps; i++)
        {
            Vector3 acc = Vector3.zero;
            acc += AddForceDummy(Physics.gravity, mass);

            position += velocity * 0.1f;
            velocity += acc * 0.1f;

            points[i] = position;
        }

        projectilePath.SetPositions(points);
    }

    Vector3 AddForceDummy(Vector3 force, float mass)
    {
        return force / mass;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(targetHitPoint.position, launchPivot.position);
    }
}
