using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private Sprite[] spriteVariations;
    
    public Movement movement { get; private set; }
    public EnemyHome home { get; private set; }
    public EnemyScatter scatter { get; private set; }
    // public EnemyChase chase { get; private set; }

    public bool isSpawned = false;

    private Transform _target;

    private EnemyBehavior initialBehavior;

    public bool debug;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        home = GetComponent<EnemyHome>();
        scatter = GetComponent<EnemyScatter>();
        // chase = GetComponent<EnemyChase>();

        initialBehavior = scatter;
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

        // chase.Disable();

        if (home != initialBehavior)
        {
            home.Disable();
        }

        if (initialBehavior != null)
        {
            initialBehavior.Enable();
        }
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