using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Movement))]
public class Player : MonoBehaviour
{

    public Movement movement { get; private set; }
    public Tilemap mistTilemap;
    public Tilemap targetTilemap;

    public float clearMistCooldownTime = 3.0f;
    private bool canClearMist = true;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
    }

    void Start()
    {
        if (mistTilemap == null || targetTilemap == null)
        {
            Debug.LogError("Tilemaps not assigned to player");
        }
    }

    void FixedUpdate()
    {
    }

    // public GameObject bulletPrefab;

    private void Update()
    {
        // prevent player update interrupts game over screen
        if (LevelManager.Instance.hasWon || LevelManager.Instance.hasLost)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement.SetDirection(Vector2.up);
        }

        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement.SetDirection(Vector2.left);
        }

        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement.SetDirection(Vector2.down);
        }

        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement.SetDirection(Vector2.right);
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

    void SweepTargets() {
        if (mistTilemap == null || targetTilemap == null) return;

        Vector3Int playerCell = mistTilemap.WorldToCell(transform.position);
        int radius = 2;

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

        Invoke(nameof(resetMistCleanCooldown), clearMistCooldownTime);
    }

    // void ShootBullet()
    // {
    //     Debug.Log("Shoot bullet");
    //     GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    //     Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
    //     bulletRb.linearVelocity = Vector2.right * 10f;
    // }

}
