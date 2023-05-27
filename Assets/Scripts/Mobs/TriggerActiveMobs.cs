using System.Collections.Generic;
using UnityEngine;

public class TriggerActiveMobs : MonoBehaviour
{
    public List<GameObject> _pools = new List<GameObject>();
    public bool _once;
    BoxCollider _boxCollider;
    [SerializeField] private int _poolChosen;

    [SerializeField] LayerMask mask;

    // private void Start()
    // {
    //     boxCollider = GetComponent<BoxCollider>();

    //     for (int i = 0; i < mobList.Count; i++)
    //     {
    //         mobList[i].SetActive(false);
    //     }
    // }

    public void GetPool()
    {
        _poolChosen = Random.Range(0, _pools.Count);
        for (int i = 0; i < _pools.Count; i++)
        {
            if (i != _poolChosen)
            {
                Destroy(_pools[i]);
            }
        }
    }

    /* private void FixedUpdate()
     {
         if (!once)
         {
             Vector3 pos = new Vector3(boxCollider.center.x, boxCollider.center.y, boxCollider.center.z);
             Vector3 size = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);

             Collider[] col = Physics.OverlapBox(pos, size / 2, Quaternion.identity, mask);

             Debug.LogError(col.Length);

             if (col.Length != 0)
                 EnableMobs();
         }
     }
     private void OnDrawGizmos()
     {
         Vector3 pos = new Vector3(transform.position.x + boxCollider.center.x,
                                   transform.position.y + boxCollider.center.y,
                                   transform.position.z + boxCollider.center.z);
         Vector3 size = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);

         Gizmos.color = Color.red;
         Gizmos.DrawWireCube(pos, size);
     }*/

    public void EnableMobs()
    {
        // int i = Random.Range(0, mobList.Count);
        // mobList[i].SetActive(true);
        _once = true;

        _pools[_poolChosen].SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_once)
        {
            EnableMobs();
        }
    }
}