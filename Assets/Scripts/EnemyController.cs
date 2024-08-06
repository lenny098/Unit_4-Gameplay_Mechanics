using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool spawnMinions = false;
    [SerializeField] float spawnRate;

    Rigidbody rigidbody;
    float destoryY;

    void SpawnMinion()
    {
        SpawnManager.Instance.SpawnMinion();
    }

    void Move()
    {
        Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        rigidbody.AddForce(direction * speed);
    }

    void DestroyOutOfBounds()
    {
        Destroy(gameObject);
        CancelInvoke("SpawnMinion");
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        destoryY = GameObject.Find("Destroy Bound").GetComponent<Renderer>().bounds.center.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (spawnMinions)
        {
            InvokeRepeating("SpawnMinion", spawnRate, spawnRate);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (transform.position.y < destoryY)
        {
            DestroyOutOfBounds();
        }
    }
}
