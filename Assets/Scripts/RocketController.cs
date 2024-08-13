using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketController : MonoBehaviour
{
    [SerializeField]  float speed;

    Rigidbody rigidbody;

    void Move()
    {
        rigidbody.AddRelativeForce(Vector3.forward * speed);
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.name)
        {
            case "Rocket Destroy Bound":
                Destroy(gameObject);

                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                Destroy(gameObject);

                break;
            default:
                break;
        }
    }
}
