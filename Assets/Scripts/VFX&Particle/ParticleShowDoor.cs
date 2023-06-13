using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class ParticleShowDoor : MonoBehaviour
{
    #region REF DOOR
    [SerializeField] Transform endDoor;
    #endregion

    #region Move
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 startUpPos;
    [SerializeField] AnimationCurve curveUpMove;
    [SerializeField] AnimationCurve curveMoveDoor;
    #endregion

    #region Destination
    Vector3 upDestination;
    [SerializeField] Vector3 doorPosition;
    [SerializeField] float upMove;
    [SerializeField] bool isUp;
    #endregion

    private void OnEnable()
    {
        GetDoorRef();
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
        startUpPos = transform.position;
        StartCoroutine(MoveDoor());
    }

    IEnumerator MoveDoor()
    {
        float time = 0;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startUpPos, doorPosition, curveMoveDoor.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
    }
}