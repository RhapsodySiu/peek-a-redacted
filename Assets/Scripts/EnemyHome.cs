using UnityEngine;

public class EnemyHome : EnemyBehavior
{
    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        
    }

}
