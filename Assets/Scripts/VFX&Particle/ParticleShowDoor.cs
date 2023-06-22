using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class ParticleShowDoor : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    float t = 0;
    Vector3 vDest;
    Vector3 origin;
    Vector3 p1;
    [SerializeField] float speed;
    float distDoor;

    bool isUp;
    bool isOnDoor;

    [Header("Trail Rotator")]
    [SerializeField] float speedRot;
    [SerializeField] float currentRot;
    [SerializeField] Transform rotator;

    #region REF DOOR
    [SerializeField] Transform endDoor;
    #endregion

    #region Move
    Vector3 startPos;
    Vector3 startUpPos;
    [SerializeField] AnimationCurve curveUpMove;
    #endregion

    #region Destination
    Vector3 upDestination;
    Vector3 doorPosition;
    [SerializeField] float upMove;
    #endregion

    private void OnEnable()
    {
        if(endDoor == null)
            GetDoorRef();

        int i = Random.Range(0, 2);
        if (i == 0)
            offset.z *= 1;
        else
            offset.z *= -1;

        transform.parent = null;
        startPos = transform.position;
        upDestination = new Vector3(startPos.x, startPos.y + upMove, startPos.z);
        doorPosition = new Vector3(endDoor.position.x, endDoor.position.y + 4, endDoor.position.z);
        StartCoroutine(UpMove());
    }

    [Button]
    void GetDoorRef()
    {
        endDoor = GameObject.Find("ExitDoor").transform;
    }

    [Button]
    IEnumerator UpMove()
    {
        float time = 0;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPos, upDestination, curveUpMove.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        isUp = true;
        startUpPos = transform.position;
        origin = startUpPos;
        p1 = (doorPosition + origin) / 2;
        p1 += offset;
        distDoor = Vector3.Distance(transform.position, doorPosition);
    }
    private void Update()
    {
        if (t <= 1 && isUp)
        {
            t += Time.deltaTime * speed / distDoor;
            transform.position = BezierCurve();
            vDest = new Vector3(doorPosition.x, doorPosition.y, doorPosition.z);
        }
        else if(isUp)
        {
            isOnDoor = true;
        }

        if(!isOnDoor)
            RotateTrail();
    }

    void RotateTrail()
    {
        currentRot -= speedRot * Time.deltaTime;
        rotator.transform.localRotation = Quaternion.Euler(0, 0, currentRot);
    }

    Vector3 BezierCurve()
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * origin;
        point += 2 * u * t * p1;
        point += tt * vDest;
        return point;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isOnDoor)
        {
            gameObject.SetActive(false);
        }
    }
}