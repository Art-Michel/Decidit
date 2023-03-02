using UnityEngine;

public class CheckPlayerCover : MonoBehaviour
{
    public static CheckPlayerCover instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
        }
    }

}
