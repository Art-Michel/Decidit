using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material_Instances : MonoBehaviour
{
    public GameObject go;
    public Color color;
    public Material material;
    // Start is called before the first frame update
    void Awake()
    {
        go = this.gameObject;
        try
        {
            material = go.GetComponent<MeshRenderer>().material;
        }
        catch
        {
            material = go.GetComponent<SkinnedMeshRenderer>().material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        material.color = color;
    }
}
