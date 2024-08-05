using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] float spawnRange;
    [SerializeField] int bossRound;

    [Header("Prefabs")]
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] GameObject[] powerupPrefabs;
    [SerializeField] GameObject bossPrefab;

    [SerializeField] GameObject rocketPrefab;

    [Header("References")]
    [SerializeField] GameObject player;

    int waveCount = 0;

    float rocketSpawnOffset = 1;
    float bossY = 0.8f;

    Vector3 RandomPosition(float spawnY = 0)
    {
        float spawnX = Random.Range(-spawnRange, spawnRange);
        float spawnZ = Random.Range(-spawnRange, spawnRange);

        return new Vector3(spawnX, spawnY, spawnZ);
    }

    void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(enemyPrefab, RandomPosition(), enemyPrefab.transform.rotation);
        }
    }

    void SpawnBosses(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(bossPrefab, RandomPosition(bossY), bossPrefab.transform.rotation);
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
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Vector3 directon = (enemy.transform.position - player.transform.position).normalized;
            Vector3 position = player.transform.position + directon * rocketSpawnOffset;

            GameObject rocket = Instantiate(rocketPrefab, position, rocketPrefab.transform.rotation);

            rocket.transform.LookAt(enemy.transform);
            rocket.transform.Rotate(Vector3.right, 90);
        }
    }

    IEnumerator SpawnRocketWaves(int waves = 2)
    {
        for (int i = 0; i < waves; i++)
        {
            SpawnRocketWave();
            yield return new WaitForSeconds(1);
        }
    }

    public void SpawnRockets()
    {
        StartCoroutine(SpawnRocketWaves());
    }
    

    // Update is called once per frame
    void Update()
    {
        float enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount < 1)
        {
            SpawnWave(++waveCount);
        }
    }
}
