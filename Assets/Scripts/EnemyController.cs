using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool spawnMinions = false;
    [SerializeField] float spawnRate;

    float destoryY = -10;

    Rigidbody rigidbody;
    GameObject player;
    SpawnManager spawnManager;

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
