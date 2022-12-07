using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float lifeTimeBullet;

    public int damageBullet;

    // Start is called before the first frame update
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ennemi"))
        {
            //collision.transform.GetComponent<AILife>().ApplyDamage(damageBullet);
        }
    }
}