using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] powerupPrefabs;
    public GameObject bossPrefab;

    public GameObject rocketPrefab;
    public GameObject player;
    
    public float spawnRange;
    public float enemyCount;

    private int waveCount = 2;
    private float rocketSpawnOffset = 1;
    private int bossRound = 5;
    private float bossY = 0.8f;

    Vector3 RandomPosition()
    {
        float spawnX = Random.Range(-spawnRange, spawnRange);
        float spawnZ = Random.Range(-spawnRange, spawnRange);

        return new Vector3(spawnX, 0, spawnZ);
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
            Vector3 position = RandomPosition();
            position.y = bossY;
            Instantiate(bossPrefab, position, bossPrefab.transform.rotation);
        }
    }

    public void SpawnMinion()
    {
        SpawnEnemies(1);
    }

    void SpawnPowerup()
    {
        // GameObject powerupPrefab = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
        GameObject powerupPrefab = powerupPrefabs[2];
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

    IEnumerator SpawnRocketWaves()
    {
        SpawnRocketWave();

        yield return new WaitForSeconds(1);

        SpawnRocketWave();
    }

    public void SpawnRockets()
    {
        StartCoroutine(SpawnRocketWaves());
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount < 1)
        {
            SpawnWave(++waveCount);
        }
    }
}
