using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("head")]
    public Transform head;
    public float mouseSensitivity = 1.2f;
    public bool invertY = false;

    [Header("launching")]
    public float throwAngle = 30.0f;
    public Transform launchPivot;
    public LineRenderer projectilePath;

    public bool allowInput;
    public Painting painting;
    public Bird birdPrefab;

    [Header("targeting")]
    public Transform targetHitPoint;
    public LayerMask hitLayerMask;
    public float radius = 0.8f;
    public float notHitForwardDistance = 5.0f;

    [Header("movement")]
    public bool run;
    public float speed = 5.0f;
    public float headHeight = 1.0f;
    public LayerMask terrainLayer;

    [Header("main cam")]
    public Camera mainCamera;

    Pool<Bird> birdPool;
    [SerializeField, NaughtyAttributes.ReadOnly]
    float xAxisClamp, yAxisClamp;

    private void Awake() {
        if(painting == null)
            painting = FindObjectOfType<Painting>();
        
        if(projectilePath == null)
            projectilePath = GetComponentInChildren<LineRenderer>();

        birdPool = new Pool<Bird>(birdPrefab, 12);
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update() {

        RotateCamera();
        
        float enter;

        RaycastHit hit;
        Ray ray = new Ray(head.position, head.forward);
        
        if(Physics.SphereCast(ray, radius, out hit, 100.0f, hitLayerMask.value))
        {
            ITarget t = hit.transform.GetComponent<ITarget>();
            if(t != null)
            {
                Plane plane = t.GetPlane();
                if(plane.Raycast(ray, out enter))
                {
                    targetHitPoint.position = ray.GetPoint(enter) + plane.normal * 0.01f;
                    targetHitPoint.rotation = LookRot(-plane.normal);
                }
            }
        }
        else
        {
            targetHitPoint.position = head.position + head.forward * notHitForwardDistance;
            targetHitPoint.rotation = LookRot(head.forward);
        }

        if(Input.GetMouseButtonDown(0))
        {
            Vector3 launch = LaunchDirection() * LaunchForce();
            if(float.IsNaN(launch.x) || float.IsNaN(launch.y) || float.IsNaN(launch.z))
            {
                Debug.Log(LaunchDirection());
                Debug.Log(LaunchForce());
                Debug.Assert(false);
            }

            Bird bird = birdPool.Get();
            Rigidbody body = bird.GetComponent<Rigidbody>();
            body.position = body.transform.position = launchPivot.position;
            body.AddForce(launch, ForceMode.VelocityChange);
            body.AddTorque(Random.onUnitSphere * 200.0f);

            body.velocity += Vector3.forward * speed;
        }

        if(run)
        {
            Vector3 next = transform.position + Vector3.forward * speed * Time.deltaTime;
            next.y = headHeight;

            if(Physics.Raycast(transform.position + Vector3.up * 5.0f, Vector3.down, out hit, 10.0f, terrainLayer.value))
                next.y += hit.point.y;
            
            transform.position = next;
        }
    }

    Quaternion LookRot (Vector3 dir)
    {
        Quaternion rot = Quaternion.LookRotation(dir, (Mathf.Abs(Vector3.Dot(dir, Vector3.up)) > 1.0f - Mathf.Epsilon) ? Vector3.right : Vector3.up);
        return rot;
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;
        yAxisClamp -= rotAmountX;

        //Vector3 targetRotCamera = transform.localRotation.eulerAngles;
        Vector3 targetRotBody = head.localRotation.eulerAngles;

        if (invertY)
            rotAmountY *= -1;

        targetRotBody.x -= rotAmountY;
        targetRotBody.z = 0;

        targetRotBody.y += rotAmountX;

        if (xAxisClamp > 90)
        {
            xAxisClamp = targetRotBody.x = 90;

        }
        else if (xAxisClamp < -90)
        {
            xAxisClamp = -90f;
            targetRotBody.x = 270f;
        }

        //transform.localRotation = Quaternion.Euler(targetRotBody);
        head.rotation = Quaternion.Euler(targetRotBody);
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
