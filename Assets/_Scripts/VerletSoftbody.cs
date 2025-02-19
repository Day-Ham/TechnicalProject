using UnityEngine;
using System.Collections.Generic;

public class VerletSoftbody : MonoBehaviour
{
    public Transform[] points; // Softbody control points
    public float stiffness = 0.1f; // How strong the softbody holds shape
    public float damping = 0.98f; // Damping factor (reduces chaos)
    public float gravity = -9.81f; // Simulated gravity

    private Vector3[] prevPositions; // Stores last frame's positions

    void Start()
    {
        // Store initial positions for Verlet calculations
        prevPositions = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            prevPositions[i] = points[i].position;
    }

    void Update()
    {
        // Apply Verlet integration on each softbody point
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 currentPos = points[i].position;
            Vector3 velocity = (currentPos - prevPositions[i]) * damping; // Compute velocity
            prevPositions[i] = currentPos; // Store old position
            points[i].position += velocity + new Vector3(0, gravity * Time.deltaTime, 0); // Apply movement & gravity
        }

        // Apply constraints to maintain shape
        ApplyConstraints();
    }

    void ApplyConstraints()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 delta = points[i + 1].position - points[i].position;
            float dist = delta.magnitude;
            float error = (dist - stiffness) * 0.5f; // Maintain equal distance
            Vector3 correction = delta.normalized * error;

            points[i].position += correction;
            points[i + 1].position -= correction;
        }
    }
}
