using UnityEngine;

[RequireComponent(typeof(Enemy))]
public abstract class EnemyBehavior : MonoBehaviour
{
    public Enemy enemy { get; private set; }
    public float duration;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public virtual void Enable()
    {
        Enable(duration);
    }

    public void Enable(float duration)
    {
        enabled = true;

        // CancelInvoke();
        // Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        enabled = false;

        // CancelInvoke();
    }
}
