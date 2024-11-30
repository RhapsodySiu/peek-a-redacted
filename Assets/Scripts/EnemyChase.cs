using UnityEngine;

public class EnemyChase : EnemyBehavior
{
    private void OnEnable()
    {
        // Debug.Log("Enable chase");
    }

    private void OnDisable()
    {
        // Debug.Log("disable chase");
        enemy.scatter.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FollowNode node = other.GetComponent<FollowNode>();

        if (node != null && enabled)
        {
            Debug.Log("Chase: enter node");
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            foreach (Vector2 availableDirection in node.availableDirections)
            {
                // if distance in this direction is less than the current
                // min distance then this direction become the new closest
                Vector3 newPos = transform.position + new Vector3(availableDirection.x, availableDirection.y);
                float distance = (enemy.GetTargetTransform().position - newPos).sqrMagnitude;

                if (distance < minDistance)
                {
                    direction = availableDirection;
                    minDistance = distance;
                }
            }

            if (enemy.movement.direction != direction)
            {
                Debug.Log($"Chase: change direction from {enemy.movement.direction} to {direction}");
            }

            enemy.movement.SetDirection(direction);
        }
    }
}
