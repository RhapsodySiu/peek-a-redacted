using System.Collections.Generic;
using UnityEngine;

public class FollowNode : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public readonly List<Vector2> availableDirections = new();

    private CircleCollider2D circleCollider;

    public bool debug = false;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        availableDirections.Clear();

        // We determine if the direction is available by box casting to see if
        // we hit a wall. The direction is added to list if available.
        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);
    }

    private void CheckAvailableDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.5f, 0f, direction, 1f, obstacleLayer);

        // If no collider is hit then there is no obstacle in that direction
        if (hit.collider == null)
        {
            availableDirections.Add(direction);
        }
        else
        {
            // Debug.Log($"Node direction {direction} not available, hit {hit.collider.gameObject.name}");
        }
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            // Draw a sphere at the node position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, GetComponent<CircleCollider2D>().radius);

            // Draw lines showing available directions
            Gizmos.color = Color.green;
            foreach (Vector2 direction in availableDirections)
            {
                // Draw line in direction
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction);

                // Draw box at end of direction to show collision check area
                Vector3 boxCenter = (Vector2)transform.position + direction;
                Vector3 boxSize = Vector3.one * 0.5f;
                Gizmos.DrawWireCube(boxCenter, boxSize);
            }

            // Draw boxes for blocked directions in red
            Gizmos.color = Color.red;
            Vector2[] allDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            foreach (Vector2 direction in allDirections)
            {
                if (!availableDirections.Contains(direction))
                {
                    Vector3 boxCenter = (Vector2)transform.position + direction;
                    Vector3 boxSize = Vector3.one * 0.5f;
                    Gizmos.DrawWireCube(boxCenter, boxSize);
                }
            }
        }
    }
}
