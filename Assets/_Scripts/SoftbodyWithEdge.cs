using UnityEngine;

public class SoftbodyWithEdge : MonoBehaviour
{
    public int pointCount = 8;
    public float radius = 1f;
    public float stiffness = 50f;
    public float damping = 2f;
    public float shapeStiffness = 5f; // Keeps points evenly spaced

    public Transform[] controlPoints;
    private Vector3[] restPositions;
    private Vector3[] velocities;
    private Vector3 prevCenter; // Track previous frame center position

    void Start()
    {
        if (controlPoints.Length != pointCount)
        {
            Debug.LogError($"Assign exactly {pointCount} control points.");
            return;
        }

        restPositions = new Vector3[pointCount];
        velocities = new Vector3[pointCount];
        prevCenter = transform.position; // Store initial position

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * Mathf.PI * 2f / pointCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            restPositions[i] = transform.position + offset;
            controlPoints[i].position = restPositions[i];
        }
    }

    void Update()
    {
        // Calculate center movement
        Vector3 centerVelocity = (transform.position - prevCenter) / Time.deltaTime;
        prevCenter = transform.position; // Update for next frame

        for (int i = 0; i < pointCount; i++)
        {
            // Update rest positions dynamically
            restPositions[i] = transform.position + (restPositions[i] - transform.position).normalized * radius;

            // Main spring force pulling points to rest positions
            Vector3 displacement = restPositions[i] - controlPoints[i].position;
            Vector3 springForce = displacement * stiffness;

            // Apply damping
            velocities[i] += springForce * Time.deltaTime;
            velocities[i] *= Mathf.Exp(-damping * Time.deltaTime);

            // Keep the points evenly spaced
            int nextIndex = (i + 1) % pointCount;
            int prevIndex = (i - 1 + pointCount) % pointCount;

            Vector3 midpoint = (controlPoints[nextIndex].position + controlPoints[prevIndex].position) * 0.5f;
            Vector3 shapeCorrection = (midpoint - controlPoints[i].position) * shapeStiffness;
            velocities[i] += shapeCorrection * Time.deltaTime;

            // Apply center movement to softbody points
            velocities[i] += centerVelocity * Time.deltaTime;

            // Move control point
            controlPoints[i].position += velocities[i] * Time.deltaTime;
        }
    }

    void OnDrawGizmos()
    {
        if (controlPoints == null) return;
        Gizmos.color = Color.green;

        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (controlPoints[i] != null)
            {
                Gizmos.DrawSphere(controlPoints[i].position, 0.05f);
                int nextIndex = (i + 1) % controlPoints.Length;
                Gizmos.DrawLine(controlPoints[i].position, controlPoints[nextIndex].position);
            }
        }
    }
}
