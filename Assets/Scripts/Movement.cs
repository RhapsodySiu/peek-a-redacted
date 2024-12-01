using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public Rigidbody2D Body { get; private set; }

    public float speed = 5.0f;
    public float speedMultiplier = 1.0f;

    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    public Vector2 direction;
    public Vector2 nextDirection;
    public Vector3 startingPosition { get; private set; }

    private Grid grid;

    public bool isPlayer = false;
    
    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
        grid = FindFirstObjectByType<Grid>();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        speedMultiplier = 1.0f;
        direction = initialDirection;
        nextDirection = Vector2.zero;
        transform.position = startingPosition;
        Body.bodyType = RigidbodyType2D.Dynamic;
        enabled = true;
    }

    private void Update()
    {
        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection, false, !isPlayer);
        }
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = Body.position + direction * speed * speedMultiplier * Time.fixedDeltaTime;
        
        Body.MovePosition(newPosition);
    }

    public void SetDirection(Vector2 dir, bool forced = false, bool waitForCenter = true)
    {
        bool shouldCheckCenter = waitForCenter;
        
        if (shouldCheckCenter && !IsAtTileCenter())
        {
            nextDirection = dir;
            return;
        }

        if (forced || !Occupied(dir))
        {
            Debug.Log($"Enemy change direction to {dir}");
            direction = dir;
            nextDirection = Vector2.zero;
            // stuckTime = 0f;
        }
        else
        {
            nextDirection = dir;
        }
    }

    public bool Occupied(Vector2 direction)
    {
        Vector2 origin = transform.position;
        Vector2 size = isPlayer ? Vector2.one * 0.7f : Vector2.one * 0.6f;
        float distance = isPlayer ? 1.0f : 1.1f;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, direction, distance, obstacleLayer);

        return hit.collider != null;
    }

    public bool IsAtTileCenter()
    {
        if (grid == null)
        {
            Debug.Log("IsAtTileCenter: Not grid reference");
        }
        if (grid == null) return false;
        
        // Convert world position to cell position
        Vector3Int cell = grid.WorldToCell(transform.position);
        // Convert cell position back to world position (center of tile)
        Vector3 center = grid.GetCellCenterWorld(cell);
        
        // Check if we're close enough to the center
        return Vector2.Distance(transform.position, center) < 0.1f;
    }

    private void OnDrawGizmos()
    {
        // Draw the BoxCast area when selected in editor
        if (direction != Vector2.zero)
        {
            Vector2 origin = transform.position;
            Vector2 size = Vector2.one * 0.7f;
            float distance = isPlayer ? 1.0f : 1.1f;

            // Draw the starting box in green
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(origin, size);

            // Draw the cast box in yellow
            Gizmos.color = Color.yellow;
            Vector2 endPosition = origin + direction * distance;
            Gizmos.DrawWireCube(endPosition, size);

            // Draw future direction box
            Gizmos.color = Color.cyan;
            Vector2 endPosition1 = origin + nextDirection * distance;
            Gizmos.DrawWireCube(endPosition1, size);

            // Draw the line between boxes
            Gizmos.DrawLine(origin, endPosition);
        }
    }
}