using UnityEngine;

public class AttractedPoint : MonoBehaviour
{
    public Transform target; // The object to attract/repel from
    public float restDistance = 2f; // Preferred distance
    public float attractionStrength = 5f; // Strength of pull when too far
    public float repulsionStrength = 10f; // Strength of push when too close
    public float bounceDamping = 0.9f; // Reduces velocity over time (0 = no damping, 1 = no movement)

    private Vector2 velocity; // Stores movement speed

    void Update()
    {
        if (target == null) return;

        Vector2 position = transform.position;
        Vector2 targetPosition = target.position;
        Vector2 direction = targetPosition - position;
        float distance = direction.magnitude;

        direction.Normalize();
        float forceMagnitude = 0f;

        if (distance > restDistance)
        {
            // Attraction force (pulls towards target)
            forceMagnitude = (distance - restDistance) * attractionStrength;
        }
        else if (distance < restDistance)
        {
            // Repulsion force (pushes away from target)
            forceMagnitude = (restDistance - distance) * -repulsionStrength;
        }

        Vector2 force = direction * forceMagnitude;

        // Apply force to velocity
        velocity += force * Time.deltaTime;

        // Dampen velocity over time (simulating friction)
        velocity *= bounceDamping;

        // Apply movement
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
