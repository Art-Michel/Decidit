using UnityEngine;

public class CheckPlayerCover : MonoBehaviour
{
    public static CheckPlayerCover instance;
    public bool isCover;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCover = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCover = false;
        }
    }

}
