using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIShoot : MonoBehaviour
{
    NavMeshAgent agent;
    NavMeshHit navHit;

    [SerializeField] Rigidbody prefabBullet;
    [SerializeField] float forceBullet, maxFireRate, fireRate;

    [SerializeField] Transform spawnBullet, playerTransform;

    [SerializeField] Vector3 targetNewPos;

    [SerializeField] bool canShoot, moveToOtherPos;

    [SerializeField] LayerMask mask;

    void Start()
    {
        spawnBullet = transform.GetChild(0);

        agent = GetComponent<NavMeshAgent>();

        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        Debug.Log("DistPlayer : " + Vector3.Distance(transform.position, playerTransform.position));

        SmoothLookAt();

        if (fireRate <= 0)
        {
            if(RaycastGun())
                Fire();
        }
        else
        {
            fireRate -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (moveToOtherPos)
        {
            targetNewPos = playerTransform.position + (Random.insideUnitSphere * Random.Range(20, 40));

            Vector3 startRayPos;
            startRayPos.x = targetNewPos.x;
            startRayPos.y = playerTransform.position.y;
            startRayPos.z = targetNewPos.z;

            Ray ray = new Ray(startRayPos, transform.TransformDirection(Vector3.down) * 10);
            RaycastHit hit;

            Debug.DrawRay(startRayPos, playerTransform.position - startRayPos, Color.green);
            Debug.DrawRay(startRayPos, Vector3.down * 100, Color.red);

            if (Physics.Raycast(ray, out hit))
            {
                targetNewPos = hit.point;
                Debug.Log(targetNewPos);
                MoveOnOtherPos();
            }
        }
    }

    void SmoothLookAt()
    {
        Vector3 relativePos = playerTransform.position - transform.position;

        relativePos.x = playerTransform.position.x - transform.position.x;
        relativePos.y = 0;
        relativePos.z = playerTransform.position.z - transform.position.z;

        Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), 0.5f);
        transform.rotation = rotation;
    }

    void Fire()
    {
        fireRate = maxFireRate;

        Rigidbody cloneBullet = Instantiate(prefabBullet, spawnBullet.position, transform.rotation);
        cloneBullet.AddRelativeForce(Vector3.forward * forceBullet, ForceMode.Impulse);
    }

    [SerializeField] float coolDownRecalcPath, maxCoolDownRecalcPath;

    bool RaycastGun() // detecte si l ennemi à le champs libre pour tirer sur le joueur 
    {
        RaycastHit hit;
        Ray ray;

        ray = new Ray(spawnBullet.position, spawnBullet.forward * 100f);

        Debug.DrawRay(spawnBullet.position, spawnBullet.TransformDirection(Vector3.forward) * 100f, Color.blue);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Player") // la vue est dégagé
            {
                if(Random.Range(0,3) ==0 && Vector3.Distance(transform.position, targetNewPos) < 2)
                {
                    moveToOtherPos = true;
                }
                return true;
            }
            else // la vue n est pas dégagé
            {
                // si aucune nouvelle position est défini, toute les "maxCoolDownRecalcPath" une nouvelle position est défini 
                if (!moveToOtherPos) 
                {
                    if(coolDownRecalcPath <= 0)
                    {
                        coolDownRecalcPath = maxCoolDownRecalcPath;
                        moveToOtherPos = true;
                    }
                    else
                    {
                        coolDownRecalcPath -= Time.deltaTime;
                    }
                }

                return false;
            }
        }

        return false;
    }

    bool DetectPointIsOnNavMesh() // detect si le point targetNewPos ce situe sur un point du nav mesh 
    {
        if (NavMesh.SamplePosition(targetNewPos, out navHit, 0.1f, NavMesh.AllAreas))
        {
            Debug.LogWarning("point on nav mesh");
            return true;
        }
        else
        {
            Debug.LogWarning("point out nav mesh");
            return false;
        }
    }

    bool DetectPointIsOpenView() // detect si le point targetNewPos à une vue dégagé sur le joueur
    {
        Vector3 adjustTargetNewPos;
        adjustTargetNewPos.x = targetNewPos.x;
        adjustTargetNewPos.y = 2.5f;
        adjustTargetNewPos.z = targetNewPos.z;

        RaycastHit hit;
        Ray ray = new Ray(adjustTargetNewPos, playerTransform.position - adjustTargetNewPos);

        Debug.DrawRay(adjustTargetNewPos, playerTransform.position - adjustTargetNewPos, Color.yellow);
        if(Physics.Raycast(ray, out hit, 100f, mask))
        {
            Debug.LogWarning(hit.transform.name);

            if (hit.transform.CompareTag("Player"))
            {
                Debug.LogWarning("view clear " + hit.transform.name);
                return true;
            }
            else
            {
                Debug.LogWarning("view not clear " + hit.transform.name);
                return false;
            }
        }

        return false;
    }

    void MoveOnOtherPos()
    {
        if(DetectPointIsOnNavMesh())
        {
            if(DetectPointIsOpenView())
            {
                moveToOtherPos = false;
                agent.SetDestination(targetNewPos);
            }
        }
        else
        {
            moveToOtherPos = true;
        }
    }
}