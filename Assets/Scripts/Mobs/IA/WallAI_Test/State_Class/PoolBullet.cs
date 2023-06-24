using System.Collections.Generic;
using UnityEngine;

public class PoolBullet : MonoBehaviour
{
    [SerializeField] List<Transform> listRB = new List<Transform>();
    [SerializeField] List<Rigidbody> listBulletAI = new List<Rigidbody>();
    [SerializeField] public int nbrBullet;

    public void CallBullet(Vector3 pos, Quaternion rot, Vector3 direction, ForceMode forceMode)
    {
        listRB[nbrBullet].position = pos;
        listRB[nbrBullet].rotation = rot;
        listRB[nbrBullet].gameObject.SetActive(true);
        listBulletAI[nbrBullet].AddRelativeForce(direction, forceMode);
        UpCountBullet();
    }
    public void CallBulletSpread(Vector3 pos, Quaternion rot, float force, ForceMode forceMode, Quaternion i, float maxAngle, float currentAgle)
    {
        listRB[nbrBullet].position = pos;
        listRB[nbrBullet].rotation = rot;
        listRB[nbrBullet].rotation = Quaternion.Euler(listRB[nbrBullet].rotation.eulerAngles.x,
                                                      listRB[nbrBullet].rotation.eulerAngles.y + currentAgle,
                                                      listRB[nbrBullet].rotation.eulerAngles.z);
        listRB[nbrBullet].gameObject.SetActive(true);
        listBulletAI[nbrBullet].AddForce(listRB[nbrBullet].forward * force, forceMode);
        UpCountBullet();
    }

    public void UpCountBullet()
    {
        if (nbrBullet < listRB.Count - 1)
            nbrBullet += 1;
        else
            nbrBullet = 0;
    }
}
