using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAI : MonoBehaviour
{
    [SerializeField] float lifeTimeBullet;

    public int damageBullet;
 
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
        Debug.Log(collider.gameObject);

        if(collider.gameObject.layer == 9)
            gameObject.SetActive(false);
    }
}
