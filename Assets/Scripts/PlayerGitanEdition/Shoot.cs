using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    RaycastHit hit;
    Ray ray;

    Transform spawnRay;

    [SerializeField] LayerMask mask;

    [SerializeField] Rigidbody prefabBullet;
    [SerializeField] float force;

    [SerializeField] bool fireClick;
    // Start is called before the first frame update
    void Start()
    {
        spawnRay = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastGun();

        if(Input.GetMouseButtonDown(0))
        {
            fireClick = true;
        }
    }
    void FixedUpdate()
    {
        if(fireClick)
        {
            Fire();
        }
    }
    void RaycastGun()
    {
        ray = new Ray(spawnRay.position, spawnRay.forward * 100f);

        Debug.DrawRay(spawnRay.position, spawnRay.TransformDirection(Vector3.forward) * 100f, Color.blue);

        if (Physics.Raycast(ray, out hit, mask))
        {
           // Debug.Log(hit.point);

            /*if (hit.transform.tag == "Ennemi")
            {
                hit.transform.GetComponent<IACAC>().CheckCanDodge();
            }*/
        }
    }

    void Fire()
    {
        fireClick = false;
        Rigidbody cloneBullet = Instantiate(prefabBullet, transform.position, transform.rotation);
        cloneBullet.AddRelativeForce(Vector3.forward * force, ForceMode.Impulse);
    }
}