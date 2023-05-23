using System.Collections;
using UnityEngine;

public class BulletAI : Health
{
    [Header("Value")]
    [SerializeField] float lifeTimeBullet;
    public int damageBullet;
    [SerializeField] bool isInEylau;
    Rigidbody rb;
    Vector3 velocityEylau;
    [SerializeField] float ratioEylau;
    [SerializeField] float rotationSpeed;
    float rotationY;
    [SerializeField] MeshRenderer meshRenderer;

    [Header("Explosion")]
    [SerializeField] GameObject vfxExplosion;
    [SerializeField] float delayBeforeExplosion;
    [SerializeField] Light lightExplosion;
    [SerializeField] Light lightTranslusance;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        //vfxExplosion = transform.GetChild(0).gameObject;
        lightExplosion.enabled = false;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine("DestroyBullet");
    }

    protected override void Update()
    {
        rotationY += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(lifeTimeBullet);
        gameObject.SetActive(false);
        yield break;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 9)
            TakeDamage(0f);

        if (collider.gameObject.name == "Spell_Eylau_Area")
        {
            Debug.Log("bullet is in" + collider.gameObject.name);

            isInEylau = true;
            velocityEylau = rb.velocity / ratioEylau;
            rb.velocity = velocityEylau;
        }
    }

    public override void TakeDamage(float damage)
    {
        Invoke("DelayBeforeExplosion", delayBeforeExplosion);
    }

    void DelayBeforeExplosion()
    {
        rb.velocity = Vector3.zero;
        lightTranslusance.enabled = false;
        meshRenderer.enabled = false;
        vfxExplosion.SetActive(true);
        Invoke("DestroyObject", 1f);
    }

    void DestroyObject()
    {
        gameObject.SetActive(false);
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