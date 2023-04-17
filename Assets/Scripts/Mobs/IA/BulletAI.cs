using System.Collections;
using UnityEngine;

public class BulletAI : Health
{
    [SerializeField] float lifeTimeBullet;
    public int damageBullet;
    [SerializeField] bool isInEylau;
    Rigidbody rb;
    Vector3 velocityEylau;

    [SerializeField] float ratioEylau;

    Health health;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine("DestroyBullet");
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(lifeTimeBullet);
        gameObject.SetActive(false);
        yield break;
    }

    private void OnTriggerEnter (Collider collider)
    {
        if(collider.gameObject.layer == 9)
            gameObject.SetActive(false);

        if(collider.gameObject.name == "Spell_Eylau_Area")
        {
            isInEylau = true;
            velocityEylau = rb.velocity / ratioEylau;
            rb.velocity = velocityEylau;
        }
    }

    public override void TakeDamage(float damage)
    {
        Debug.Log("Explosion");
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Spell_Eylau_Area")
        {
            isInEylau = false;
            velocityEylau = rb.velocity * ratioEylau;
            rb.velocity = velocityEylau;
        }
    }
}