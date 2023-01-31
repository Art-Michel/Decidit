using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    #region Variables
    [SerializeField] bool isEntree;

    [SerializeField] GameObject _Door;

    [SerializeField] List<GameObject> _roomNb;

    int _roomIndex = 0;
    [SerializeField] int _nbIA;

    #endregion

    void Awake()
    {

    }

    void Start()
    {
        _roomNb = DungeonGenerator.Instance.GetRoom();
    }

    void Update()
    {
        _nbIA = CheckIA();
    }

    #region Functions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isEntree)
            {
                _Door.GetComponent<BoxCollider>().enabled = false;
                _Door.GetComponent<MeshRenderer>().enabled = false;
                _roomIndex++;
            }

            if (!isEntree)
            {
                if (_nbIA == 0)
                {
                    _Door.GetComponent<BoxCollider>().enabled = false;
                    _Door.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isEntree)
            {
                _Door.GetComponent<BoxCollider>().enabled = true;
                _Door.GetComponent<MeshRenderer>().enabled = true;
            }

            if (!isEntree)
            {
                if (_nbIA == 0)
                {
                    _Door.GetComponent<BoxCollider>().enabled = true;
                    _Door.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }

    public int CheckIA()
    {
        return (GameObject.FindGameObjectsWithTag("Ennemi").Length);
    }

    #endregion
}
