using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullCount : MonoBehaviour
{
    [SerializeField] List<Transform> listBull = new List<Transform>();

    void Start()
    {
        AddBull();
    }

    void AddBull()
    {
        if(transform.childCount>0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                listBull.Add(transform.GetChild(i));
            }
        }
    }

    public void RemoveAI(Transform _transform)
    {
        listBull.Remove(_transform);
    }
}