using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    private float speed = 10;
    private float bound = 100;

    private Rigidbody rigidbody;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (transform.position - player.transform.position).normalized;
        rigidbody.AddForce(direction * speed);

        if (Vector3.Distance(transform.position, player.transform.position) > bound)
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
