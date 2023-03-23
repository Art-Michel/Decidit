using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class Integration : MonoBehaviour
{
    public List<Transform> badAsset = new List<Transform>();
    public List<Transform> goodAsset = new List<Transform>();

    public int nbrInstance;
    public GameObject objToCreate;

    public string nameObj;

    [Button]
    void DuplicateObj()
    {
        for(int i=0; i< nbrInstance; i ++)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(objToCreate) as GameObject;
            go.transform.parent = gameObject.transform.parent;
            goodAsset.Add(go.transform);
        }
    }

    [Button]
    void PlaceAsset()
    {
        for(int i =0; i < badAsset.Count; i ++)
        {
            goodAsset[i].position = badAsset[i].position;
            goodAsset[i].rotation = badAsset[i].rotation;
            goodAsset[i].localScale = badAsset[i].localScale;

            goodAsset[i].GetChild(1).gameObject.SetActive(false);
            goodAsset[i].GetChild(2).gameObject.SetActive(false);
        }
    }

    [Button]
    void DisableBadAsset()
    {
        for (int i = 0; i < badAsset.Count; i++)
        {
            badAsset[i].gameObject.SetActive(false);
        }
    }

    [Button]
    void GetAssetByName()
    {
        int j = 0;
        for (int i =0; i < transform.childCount; i ++)
        {
            if(transform.GetChild(i).name.Contains(nameObj))
            {
                badAsset.Add(transform.GetChild(i));
                transform.GetChild(i).transform.SetSiblingIndex(j);
                j++;
            }
        }
    }

}
