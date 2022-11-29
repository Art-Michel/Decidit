using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPoint : MonoBehaviour
{
    [SerializeField] GameObject [] walls;
    Transform playerTransform;
    [SerializeField] LayerMask mask;
    RaycastHit hit;

    [SerializeField] float rightLenght;
    [SerializeField] float leftLenght;
    [SerializeField] float forwardLenght;
    [SerializeField] float backLenght;
    [SerializeField] Vector3 newPos;

    [SerializeField] float min;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        walls = GameObject.FindGameObjectsWithTag("WallAI");
    }

    // Update is called once per frame
    void Update()
    {
        float min1 = Mathf.Min(forwardLenght, backLenght);
        float min2 = Mathf.Min(rightLenght, leftLenght);

        min = Mathf.Min(min1, min2);
    }

    Vector3 PlayerPos()
    {
        newPos.x = Random.Range(-walls[0].transform.localPosition.x, walls[0].transform.localPosition.x);
        newPos.y = Random.Range(-walls[0].transform.localPosition.y, walls[0].transform.localPosition.y);
        newPos.z = 0;

        return newPos;
    }
}