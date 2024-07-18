using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool spawnMinions = false;
    private float spawnRate = 5;
    private float destoryY = -10;

    private Rigidbody rigidbody;
    private GameObject player;
    private SpawnManager spawnManager;

    void SpawnMinion()
    {
        spawnManager.SpawnMinion();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if (spawnMinions)
        {
            InvokeRepeating("SpawnMinion", spawnRate, spawnRate);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        rigidbody.AddForce(direction * speed);

        if (transform.position.y < destoryY)
        {
            Destroy(gameObject);
            CancelInvoke();
        }
    }
}
