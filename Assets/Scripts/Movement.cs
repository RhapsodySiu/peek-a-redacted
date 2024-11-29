using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public Rigidbody2D Body { get; private set; }

    public float speed = 5.0f;
    public float speedMultiplier = 1.0f;

    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    public Vector2 direction;
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
    }

    private void Start()
    {
        ResetState();
    }

    private void ResetState()
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
            SetDirection(nextDirection);
        }
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = Body.position + direction * speed * speedMultiplier * Time.fixedDeltaTime;

        Body.MovePosition(newPosition);
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            nextDirection = Vector2.zero;
        }
        else
        {
            nextDirection = direction;
        }
    }

    // Helper method to draw a box using Debug.DrawLine
    private void DrawBox(Vector2 position, Vector2 size, Color color)
    {
        Vector2 halfSize = size * 0.5f;

        // Draw the box edges using Debug.DrawLine
        // Bottom line
        Debug.DrawLine(new Vector2(position.x - halfSize.x, position.y - halfSize.y),
                    new Vector2(position.x + halfSize.x, position.y - halfSize.y),
                    color,
                    0.1f);
        // Top line
        Debug.DrawLine(new Vector2(position.x - halfSize.x, position.y + halfSize.y),
                    new Vector2(position.x + halfSize.x, position.y + halfSize.y),
                    color,
                    0.1f);
        // Left line
        Debug.DrawLine(new Vector2(position.x - halfSize.x, position.y - halfSize.y),
                    new Vector2(position.x - halfSize.x, position.y + halfSize.y),
                    color,
                    0.1f);
        // Right line
        Debug.DrawLine(new Vector2(position.x + halfSize.x, position.y - halfSize.y),
                    new Vector2(position.x + halfSize.x, position.y + halfSize.y),
                    color,
                    0.1f);
    }

    public bool Occupied(Vector2 direction)
    {
        Vector2 origin = transform.position;
        Vector2 size = Vector2.one * 0.55f;
        float distance = 1.5f;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, direction, distance, obstacleLayer);

        // Visualization code
        Color boxColor = hit.collider != null ? Color.red : Color.green;

        // Draw the box at the start position
        DrawBox(origin, size, boxColor);

        // Draw the box at the end position
        DrawBox(origin + direction * distance, size, boxColor);

        // Draw a line connecting the centers of the boxes
        Debug.DrawLine(origin, origin + direction * distance, boxColor);

        return hit.collider != null;
    }
}