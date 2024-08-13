using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] GameObject spawnBound;
    [SerializeField] GameObject spawnGround;
    [SerializeField] int bossRound;

    [Header("Rockets")]
    [SerializeField] float rocketSpawnOffset;
    [SerializeField] int rocketWaves;
    [SerializeField] float rocketWaveInterval;

    [Header("Prefabs")]
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] GameObject[] powerupPrefabs;
    [SerializeField] GameObject bossPrefab;
    [SerializeField] GameObject rocketPrefab;

    int waveCount = 0;
    float spawnGroundY;

    Vector3 RandomPosition(float spawnY = 0)
    {
        // The bound should have same size in X and Z
        float spawnBoundRadius = spawnBound.GetComponent<Renderer>().bounds.extents.x;
        Vector2 randomPositionInBound = Random.insideUnitCircle * spawnBoundRadius;

        return (
            spawnBound.transform.position +
            new Vector3(randomPositionInBound.x, spawnY, randomPositionInBound.y)
        );
    }

    float CalculateSpawnY(GameObject prefab)
    {
        float radius = prefab.GetComponent<Renderer>().bounds.extents.x;

        return spawnGroundY + radius;
    }

    void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            Vector3 spawnPosition = RandomPosition(spawnY: CalculateSpawnY(enemyPrefab));

            Instantiate(enemyPrefab, spawnPosition, enemyPrefab.transform.rotation);
        }
    }

    void SpawnBosses(int count)
    {
        float bossSpawnY = CalculateSpawnY(bossPrefab);

        for (int i = 0; i < count; i++)
        {
            Instantiate(bossPrefab, RandomPosition(spawnY: bossSpawnY), bossPrefab.transform.rotation);
        }
    }

    public void SpawnMinion()
    {
        SpawnEnemies(1);
    }

    void SpawnPowerup()
    {
        GameObject powerupPrefab = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];

        Instantiate(powerupPrefab, RandomPosition(), powerupPrefab.transform.rotation);
    }

    void SpawnWave(int count)
    {
        if (count % bossRound == 0)
        {
            SpawnBosses(count / bossRound);
        }
        else
        {
            SpawnEnemies(count);
        }
        
        SpawnPowerup();
    }

    void SpawnRocketWave()
    {
        Vector3 playerPosition = PlayerController.Instance.transform.position;

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Vector3 directon = (enemy.transform.position - playerPosition).normalized;
            Vector3 position = playerPosition + directon * rocketSpawnOffset;

            Instantiate(rocketPrefab, position, Quaternion.LookRotation(directon));
        }
    }

    IEnumerator SpawnRocketWaves(int waves)
    {
        for (int i = 0; i < waves; i++)
        {
            SpawnRocketWave();
            yield return new WaitForSeconds(rocketWaveInterval);
        }
    }

    public void SpawnRockets()
    {
        StartCoroutine(SpawnRocketWaves(rocketWaves));
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        spawnGroundY = spawnGround.GetComponent<Renderer>().bounds.max.y;
    }

    // Update is called once per frame
    void Update()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount < 1)
        {
            SpawnWave(++waveCount);
        }
    }
}
