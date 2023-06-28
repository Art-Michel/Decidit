using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

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
    [SerializeField] VisualEffect VFX;
    [SerializeField] GameObject VFXObject;

    [Header("Explosion")]
    [SerializeField] GameObject vfxExplosion;
    [SerializeField] float delayBeforeExplosion;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        VFXObject.SetActive(true);
        StartCoroutine("DestroyBullet");
    }

    protected override void Update()
    {
       /* rotationY += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);*/
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

    public override bool TakeDamage(float damage)
    {
        Invoke("DelayBeforeExplosion", delayBeforeExplosion);
        return true;
    }

    void DelayBeforeExplosion()
    {
        rb.velocity = Vector3.zero;
        VFX.Stop();
        vfxExplosion.SetActive(true);
        Invoke("DisableObject", 0.001f);
    }

    void DisableObject()
    {
        VFXObject.SetActive(false);
        Invoke("disableVFXExplosion", 1f);
    }

    void disableVFXExplosion()
    {
        vfxExplosion.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
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