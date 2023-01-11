using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public static Doors Instance = null;
    [SerializeField] int NbIA;

    [SerializeField] GameObject Door;
    [SerializeField] List<GameObject> IA;

    private void Awake()
    {
        if(Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
        NbIACheck();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(NbIA <= 0)
        {
            Debug.Log("Ouverture");
        }
    }

    private void NbIACheck()
    {
        NbIA = GameObject.FindGameObjectsWithTag("Ennemi").Length;
    }
    public void NbIASubqtract()
    {
        NbIA -= 1;
    }
}
