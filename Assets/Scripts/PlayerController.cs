using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public bool onGround = false;

    public bool poweredUp = false;
    public float powerupForce;
    public float powerupDuration;
    public GameObject powerupIndicator;

    public bool smash = false;
    // public float smashDistanceScale;
    public float smashForce;
    public float smashRadius;
    public GameObject smashIndicator;

    private Rigidbody rigidbody;
    public GameObject focalPoint;
    
    public GameObject spawnManager;

    private Vector3 indicatorOffset = new Vector3(0, -0.5f, 0);

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        rigidbody.AddForce(focalPoint.transform.forward * verticalInput * speed);

        // Indicators moves with player
        powerupIndicator.transform.position = transform.position + indicatorOffset;
        smashIndicator.transform.position = transform.position + indicatorOffset;

        if (onGround && smash && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Smash());
        }
    }

    IEnumerator PowerupCountdown()
    {
        poweredUp = true;
        powerupIndicator.gameObject.SetActive(true);

        yield return new WaitForSeconds(powerupDuration);

        poweredUp = false;
        powerupIndicator.gameObject.SetActive(false);
    }

    IEnumerator SmashCountdown()
    {
        smash = true;
        smashIndicator.gameObject.SetActive(true);

        yield return new WaitForSeconds(powerupDuration);

        smash = false;
        smashIndicator.gameObject.SetActive(false);
    }

    IEnumerator Smash()
    {
        rigidbody.AddForce(Vector3.up * 30, ForceMode.Impulse);
        onGround = false;

        yield return new WaitForSeconds(0.2f);

        rigidbody.AddForce(Vector3.down * 80, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Powerup":
                Destroy(other.gameObject);

                StartCoroutine(PowerupCountdown());

                break;
            case "Rockets":
                spawnManager.GetComponent<SpawnManager>().SpawnRockets();

                Destroy(other.gameObject);

                break;
            case "Smash":
                Destroy(other.gameObject);

                StartCoroutine(SmashCountdown());

                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        switch (other.tag)
        {
            case "Enemy":
                if (poweredUp)
                {
                    Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();

                    Vector3 direction = (other.transform.position - transform.position).normalized;

                    // Debug.Log($"Collied with {other.name}, while poweredUp is {poweredUp}");
                    enemyRigidbody.AddForce(direction * powerupForce, ForceMode.Impulse);
                }

                break;
            case "Ground":
                onGround = true;

                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (var enemy in enemies)
                {
                    Rigidbody enemyRigidbody = enemy.GetComponent<Rigidbody>();

                    // Vector3 direction = (enemy.transform.position - transform.position).normalized;
                    // float distance = Vector3.Distance(enemy.transform.position, transform.position);

                    // enemyRigidbody.AddForce(direction * (smashDistanceScale / distance) * smashForce, ForceMode.Impulse);

                    enemyRigidbody.AddExplosionForce(smashForce, transform.position, smashRadius, 0, ForceMode.Impulse);
                }

                break;
            default:
                break;
        }
    }
}
