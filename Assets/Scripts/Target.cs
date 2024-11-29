using UnityEngine;

public class Target : MonoBehaviour
{
    public int points = 10;
    private SpriteRenderer _renderer;

    private bool detected = false;

    public void Start() {
        _renderer = GetComponent<SpriteRenderer>();

        _renderer.enabled = false;
    }

    public void Spot() {
        Debug.Log("Spotted");
        detected = true;
        _renderer.enabled = true;
    }

    private void Find()
    {
        LevelManager.Instance.ItemFound(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered with: " + other.gameObject.name);
        if (detected && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Item found by player");
            Find();
        }
    }
}