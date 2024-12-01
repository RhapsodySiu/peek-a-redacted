using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private Sprite[] spriteVariations;
    
    public Movement movement { get; private set; }
    public EnemyHome home { get; private set; }
    public EnemyScatter scatter { get; private set; }

    public bool isSpawned = false;

    private Transform _target;
    private BoxCollider2D _collider;

    public bool debug;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        scatter = GetComponent<EnemyScatter>();

        _collider = GetComponent<BoxCollider2D>();
    }
    
    private void OnDrawGizmos()
    {
        if (_collider != null)
        {
            // Draw the box collider area
            Gizmos.color = Color.magenta;
            Vector2 center = _collider.bounds.center;
            Vector2 size = _collider.bounds.size;
            Gizmos.DrawWireCube(center, size);
        }
    }

    public Transform GetTargetTransform()
    {
        return _target.transform;
    }

    public void SetTarget(Transform t)
    {
        _target = t;
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
        movement.ResetState();
    }

    void OnValidate()
    {
        if (spriteVariations.Length == 0)
        {
            Debug.LogWarning("Enemy variation is not set");
        }
    }

    void Start() {
        var renderer = GetComponentInChildren<SpriteRenderer>();

        if (renderer != null && spriteVariations.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, spriteVariations.Length);
            renderer.sprite = spriteVariations[randomIndex];
        }

        if (debug)
        {
            ResetState();
            _target = FindFirstObjectByType<Player>().transform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Enemy collides with Player");

            LevelManager.Instance.Lose();
        }
    }
}