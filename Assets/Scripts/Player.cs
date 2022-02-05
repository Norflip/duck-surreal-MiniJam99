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
    public Transform targetHitPoint;
    public Transform launchPivot;
    public LineRenderer projectilePath;

    public bool allowInput;
    public Painting painting;

    public Bird birdPrefab;

    [Header("movement")]
    public bool run;
    public float speed = 5.0f;
    public float headHeight = 1.0f;
    public LayerMask terrainLayer;


    float xAxisClamp;

    private void Awake() {
        if(painting == null)
            painting = FindObjectOfType<Painting>();
        
        if(projectilePath == null)
            projectilePath = GetComponentInChildren<LineRenderer>();

        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update() {

        RotateCamera();

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

        if(run)
        {
            Vector3 next = transform.position + Vector3.forward * speed * Time.deltaTime;
            next.y = headHeight;

            if(Physics.Raycast(transform.position + Vector3.up * 5.0f, Vector3.down, out RaycastHit hit, 10.0f, terrainLayer.value))
                next.y += hit.point.y;
            
            transform.position = next;
        }
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;

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
