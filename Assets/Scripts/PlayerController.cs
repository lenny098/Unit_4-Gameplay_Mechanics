using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] float speed;
    [SerializeField] float powerupDuration;

    [Header("Knockback Powerup")]
    [SerializeField] float knockbackForce;
    [SerializeField] GameObject knockbackIndicator;

    [Header("Smash Powerup")]
    [SerializeField] KeyCode smashKey;
    [SerializeField] float smashY;
    [SerializeField] float smashUpwardSpeed;
    [SerializeField] float smashDownwardSpeed;
    [SerializeField] float smashForce;
    [SerializeField] float smashRadius;
    [SerializeField] GameObject smashIndicator;

    [Header("References")]
    [SerializeField] GameObject focalPoint;

    // Private variables
    Rigidbody rigidbody;

    bool withKnockback = false;
    bool withSmash = false;
    bool withPowerUp { get { return withKnockback || withSmash; } }

    bool isSmashing = false;
    bool isMovingUpward = false;

    void MovePlayer()
    {
        float verticalInput = Input.GetAxis("Vertical");
        rigidbody.AddForce(focalPoint.transform.forward * verticalInput * speed);
    }

    void MoveIndicators()
    {
        GameObject[] indicators = new GameObject[] { knockbackIndicator, smashIndicator };

        foreach (var indicator in indicators)
        {
            indicator.transform.position = new Vector3(
                transform.position.x,
                indicator.transform.position.y,
                transform.position.z
            );
        }
    }

    IEnumerator KnockbackCountdown()
    {
        withKnockback = true;
        knockbackIndicator.SetActive(true);

        yield return new WaitForSeconds(powerupDuration);

        withKnockback = false;
        knockbackIndicator.SetActive(false);
    }

    IEnumerator SmashCountdown()
    {
        withSmash = true;
        smashIndicator.SetActive(true);

        yield return new WaitForSeconds(powerupDuration);

        withSmash = false;
        smashIndicator.SetActive(false);
    }

    void StartSmash()
    {
        isSmashing = true;
        isMovingUpward = true;
    }

    void Smashing()
    {
        if (isMovingUpward)
        {
            transform.position += Vector3.up * Time.deltaTime * smashUpwardSpeed;  
        }
        else // Downward
        {
            transform.position -= Vector3.up * Time.deltaTime * smashDownwardSpeed;
        }

        if (transform.position.y >= smashY)
        {
            isMovingUpward = false;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        MoveIndicators();

        if (withSmash && !isSmashing && Input.GetKeyDown(smashKey))
        {
            StartSmash();
        }

        if (isSmashing)
        {
            Smashing();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Powerup": // Knockback
                if (withPowerUp) break;

                Destroy(other.gameObject);
                StartCoroutine(KnockbackCountdown());

                break;
            case "Rockets":
                Destroy(other.gameObject);
                SpawnManager.Instance.SpawnRockets();

                break;
            case "Smash":
                if (withPowerUp) break;

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
                if (!withKnockback) break;

                Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
                Vector3 direction = (other.transform.position - transform.position).normalized;

                otherRigidbody.AddForce(direction * knockbackForce, ForceMode.Impulse);

                break;
            case "Ground":
                if (!isSmashing) break;
                isSmashing = false;

                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (var enemy in enemies)
                {
                    Rigidbody enemyRigidbody = enemy.GetComponent<Rigidbody>();

                    enemyRigidbody.AddExplosionForce(smashForce, transform.position, smashRadius, 0, ForceMode.Impulse);
                }

                break;
            default:
                break;
        }
    }
}
