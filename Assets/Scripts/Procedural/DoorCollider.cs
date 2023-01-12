using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //si c'est la porte du bout du coulloir...
            if (Doors.Instance.GetDoorIndex() % 2 == 1)
            {
                Doors.Instance.GetDoor().SetActive(false);
                Doors.Instance.GetRoom().SetActive(true);
                Doors.Instance.AddRoomIndex();
                Doors.Instance.AddDoorIndex();
                Doors.Instance.NbIACheck();
                gameObject.SetActive(false);
            }
            else
            {
                //on ferme la parte derière le joueur et on enlève la room qu'il vient de passer
                Doors.Instance.GetDoor().SetActive(true);
                Doors.Instance.GetRoom().SetActive(false);
                Doors.Instance.AddRoomIndex();
                gameObject.SetActive(false);
                Doors.Instance.AddDoorIndex();
            }
        }
    }
}
