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
    MeshRenderer meshRenderer;

    [Header("Explosion")]
    GameObject vfxExplosion;
    [SerializeField] float delayBeforeExplosion;


    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        vfxExplosion = transform.GetChild(0).gameObject;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine("DestroyBullet");
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
            gameObject.SetActive(false);

        if (collider.gameObject.name == "Spell_Eylau_Area")
        {
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