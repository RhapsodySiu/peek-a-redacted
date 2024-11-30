using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Movement))]
public class Player : MonoBehaviour
{

    public Movement movement { get; private set; }
    public Tilemap mistTilemap;
    public Tilemap targetTilemap;
    private AudioSource sweepAudioSource;
    private SpriteRenderer _renderer;
    private Collider2D _collider;

    public float clearMistCooldownTime = 1.5f;
    private bool canClearMist = true;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        sweepAudioSource = GetComponent<AudioSource>();
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        movement.isPlayer = true;
    }

    void FixedUpdate()
    {
    }

    private void Update()
    {
        // prevent player update interrupts game over screen
        if (LevelManager.Instance.hasWon || LevelManager.Instance.hasLost)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement.SetDirection(Vector2.up, false, false);
        }

        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement.SetDirection(Vector2.left, false, false);
        }

        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement.SetDirection(Vector2.down, false, false);
        }

        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement.SetDirection(Vector2.right, false, false);
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canClearMist) {
                canClearMist = false;
                SweepTargets();
            }
        }

        float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);

    }

    private void resetMistCleanCooldown() {
        canClearMist = true;
    }

    public void ResetState()
    {
        enabled = true;
        _renderer.enabled = true;
        _collider.enabled = true;
        
        movement.ResetState();
        gameObject.SetActive(true);
    }

    public void Die()
    {
        enabled = false;
        _renderer.enabled = false;
        _collider.enabled = false;
        movement.enabled = false;
    }

    void SweepTargets() {
        if (mistTilemap == null || targetTilemap == null) return;

        Vector3Int playerCell = mistTilemap.WorldToCell(transform.position);
        int radius = 3;

        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                Vector3Int tilePosition = playerCell + new Vector3Int(x, y, 0);
                float distance = Mathf.Sqrt(x*x + y*y);

                if (distance <= radius) {
                    // Clear mist
                    mistTilemap.SetTile(tilePosition, null);

                    // Get world position of the tile center
                    Vector3 worldPos = targetTilemap.GetCellCenterWorld(tilePosition);
                    
                    // Check for Target object at this position
                    Collider2D[] colliders = Physics2D.OverlapPointAll(worldPos);
                    foreach (Collider2D collider in colliders) {
                        Target target = collider.GetComponent<Target>();
                        if (target != null) {
                            target.Spot();
                        }
                    }
                }
            }
        }

        LevelManager.Instance.TrySpawnNewEnemy();

        sweepAudioSource?.Play();

        Invoke(nameof(resetMistCleanCooldown), clearMistCooldownTime);
    }

    private void OnDrawGizmos()
    {
        if (_collider != null)
        {
            // Get the collider bounds
            Bounds bounds = _collider.bounds;
            
            // Set gizmo color
            Gizmos.color = Color.yellow;
            
            // Draw wire cube representing the collider
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }

}
