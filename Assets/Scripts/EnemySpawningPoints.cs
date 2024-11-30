using UnityEngine;

public struct SpawnInfo {
    public Vector3 position;
    public Vector2 direction;
}
public class EnemySpawningPoints : MonoBehaviour
{
    public Transform[] positions;
    public Vector2[] directions;

    public SpawnInfo? GetSpawnPosition()
    {
        if (positions.Length == 0 || positions.Length != directions.Length)
        {
            return null;
        }

        int idx = Random.Range(0, positions.Length);

        return new SpawnInfo{ position = positions[idx].position, direction = directions[idx] };
    }
}
