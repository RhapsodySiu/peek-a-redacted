using UnityEngine;

public class EnemyScatter : EnemyBehavior
{
    private Vector2? pendingDirection = null;

    private void OnDisable()
    {
        Debug.Log("disable scatter");
        // enemy.chase.Enable();
    }

    private void OnEnable()
    {
        // Debug.Log("enable scatter");
    }

    private void Update()
    {
        if (pendingDirection.HasValue && enemy.movement.IsAtTileCenter())
        {
            enemy.movement.SetDirection(pendingDirection.Value, waitForCenter: true);
            pendingDirection = null;

            Debug.Log("clear pending direction");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FollowNode node = other.GetComponent<FollowNode>();

        Debug.Log($"Hit {LayerMask.LayerToName(other.gameObject.layer)} enabled {enabled}");

        if (node != null && enabled)
        {
            int index = Random.Range(0, node.availableDirections.Count);

            Debug.Log($"idx {index}");

            if (node.availableDirections.Count > 1 && node.availableDirections[index] == -enemy.movement.direction)
            {
                index++;
                if (index >= node.availableDirections.Count)
                {
                    index = 0;
                }
            }

            Debug.Log($"final {index}");
            pendingDirection = node.availableDirections[index];
            Debug.Log($"Hit trigger node direction {pendingDirection}");
        }
    }
}
