using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketController : MonoBehaviour
{
    [SerializeField]  float speed = 10;
    [SerializeField] float bound = 100;

    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = PlayerController.Instance.transform.position;

        Vector3 direction = (transform.position - playerPosition).normalized;
        rigidbody.AddForce(direction * speed);

        if (Vector3.Distance(transform.position, playerPosition) > bound)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        switch (other.tag)
        {
            case "Enemy":
                Destroy(gameObject);

                break;
            default:
                break;
        }
    }
}
