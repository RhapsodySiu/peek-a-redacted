using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;


public class LevelManager : MonoBehaviour
{
    const int MAX_LIVES = 3;

    public static LevelManager Instance { get; private set; }

    private LevelUI _currentLevelUI;
    private Player _player;
    private List<Enemy> _enemies;
    private Transform _targets;

    public int currentLevel;

    public bool hasWon;
    public bool hasLost;

    public bool debugLevel;
    public bool debugAddEnemy;

    public int score { get; private set; }

    public int remainingLives;
    public int targetsFound;

    [SerializeField] private GameObject enemyPrefab;

    private AudioSource _audioSource;
    [SerializeField] private AudioResource scoreFx;
    [SerializeField] private AudioResource hurtFx;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        hasWon = false;
        hasLost = false;

        if (!debugLevel)
            currentLevel = 0;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Fix strange collision detection issue
    private void hackFix()
    {
        _player.transform.localScale = new Vector3(0.8f, 0.8f, 0f);

        foreach (Enemy enemy in _enemies)
        {
            enemy.transform.localScale = new Vector3(0.72f, 0.72f, 0f);
        }
    }

    void SetLives(int lives)
    {
        remainingLives = lives;
    }

    void OnValidate()
    {
        if (enemyPrefab == null)
            Debug.LogError("Enemy prefab not set in level manager");
    }

    void Update()
    {
        if (debugAddEnemy)
        {
            debugAddEnemy = false;
            spawnNewEnemy();
        }
    }

    void ResetState()
    {
        // Update player tile map reference to mist
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);

        // Debug.Log($"TileMap count {tilemaps.Length}");
        Time.timeScale = 1;

        if (_player != null)
        {
            _player.ResetState();
            foreach (Tilemap tilemap in tilemaps)
            {
                int layer = tilemap.gameObject.layer;

                if (layer == LayerMask.NameToLayer("Mist"))
                {
                    _player.mistTilemap = tilemap;
                }
                else if (layer == LayerMask.NameToLayer("Target"))
                {
                    _player.targetTilemap = tilemap;
                }
            }

            if (_player.mistTilemap == null)
            {
                Debug.LogError("cannot find mist tilemap in player");
            }
        }
        foreach (Enemy enemy in _enemies)
        {
            enemy.ResetState();
            enemy.SetTarget(_player.transform);
        }
    }

    public void setLevel(int id)
    {
        currentLevel = id;
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        Debug.Log($"Debug LevelManager: should start new game? {debugLevel}");
        if (debugLevel)
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        Debug.Log("LevelManager: new game");
        hasLost = false;
        hasWon = false;
        targetsFound = 0;
        SetLives(MAX_LIVES);

        // Get dependencies
        Grid grid = FindFirstObjectByType<Grid>();
        LevelUI levelUI = FindFirstObjectByType<LevelUI>();

        _enemies = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
        _player = FindFirstObjectByType<Player>();

        if (_player == null)
            Debug.LogError("LevelManager cannot find player");

        if (grid)
        {
            Transform targets = grid.transform.Find("Target");

            if (targets != null)
            {
                _targets = targets;
            }
            else
            {
                Debug.LogError("NewGame: Fail to locate Targets");
            }
        }
        else
        {
            Debug.LogError("NewGame: Fail to locate Grid");
        }
        if (levelUI)
        {
            _currentLevelUI = levelUI;
            _currentLevelUI.fillHearts(MAX_LIVES);
            _currentLevelUI.UpdateItemText(targetsFound, _targets.childCount);
        }
        else
        {
            Debug.LogError("NewGame: Fail to locate level UI");
        }

        ResetState();

        hackFix();
    }

    void WinGame()
    {
        Debug.Log("WinGame");
        hasWon = true;
        // TODO: Play successful animation and move to next scene
        GameManager.Instance.pauseAudio();

        if (_currentLevelUI != null)
        {
            // Freeze the game
            Time.timeScale = 0;

            if (_player)
            {
                _player.GetComponent<Movement>().enabled = false;
            }

            foreach (Enemy enemy in _enemies)
            {
                enemy.gameObject.SetActive(false);
            }

            _currentLevelUI.showVictoryScreen();
        }
        else
        {
            Debug.LogError("Cannot find active canvas in the scene");
        }
        // Invoke(nameof(Callback), 3.0f);
    }

    public void Lose()
    {
        _player.Die();
        _currentLevelUI.TriggerFlash();

        _audioSource.resource = hurtFx;
        _audioSource.Play();
        
        Debug.Log("Player lost");

        SetLives(remainingLives - 1);

        _currentLevelUI?.subtractHeart();
        // if live > 0 show button that reset state
        if (remainingLives > 0)
        {
            Invoke(nameof(ResetState), 3f);
        }
        else
        {
            GameManager.Instance.pauseAudio();
            hasLost = true;

            if (_player)
            {
                _player.gameObject.SetActive(false);
            }

            _currentLevelUI?.showGameOverScreen();
        }
    }

    public void HandleSweep(float cooldownTime)
    {
        _currentLevelUI?.ResetCooldown(cooldownTime);

        if (Random.value < 0.9f)
        {
            return;
        }
        
        spawnNewEnemy();
    }

    private void spawnNewEnemy()
    {
        if (enemyPrefab == null) {
            Debug.LogError("Could not find Enemy prefab in Resources folder");
            return;
        }

        EnemySpawningPoints spawn = FindFirstObjectByType<EnemySpawningPoints>();
        SpawnInfo? spawnInfo = spawn?.GetSpawnPosition();
        
        if (spawnInfo != null)
        {
            // Instantiate the enemy at a random position
            Vector3 randomPosition = spawnInfo.Value.position;
            GameObject enemyObj = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
            
            // Get the Enemy component
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy == null) {
                Debug.LogError("Instantiated enemy prefab does not have Enemy component");
                return;
            }

            enemy.isSpawned = true;
            enemy.movement.initialDirection = spawnInfo.Value.direction;
            enemy.ResetState();
            enemy.SetTarget(_player.transform);

            _enemies.Add(enemy);
        
            hackFix();
        }
    }

    private void SetScore(int newScore)
    {
        Debug.Log($"SetScore from {score} to {newScore}");
        score = newScore;

        if (_currentLevelUI != null)
        {
            _currentLevelUI.scoreText.text = $"Score: {score}";
        }
    }

    public void ItemFound(Target target)
    {
        if (scoreFx != null && _audioSource != null)
        {
            _audioSource.resource = scoreFx;
            _audioSource.Play();
        }

        target.gameObject.SetActive(false);
        targetsFound++;
        _currentLevelUI?.UpdateItemText(targetsFound, _targets.childCount);
        
        SetScore(score + target.points);

        if (!HasRemainingTargets())
        {
            WinGame();
        }
    }
    private bool HasRemainingTargets()
    {
        foreach (Transform target in _targets)
        {
            if (target.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }
}
