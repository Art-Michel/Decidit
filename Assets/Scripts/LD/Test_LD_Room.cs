using UnityEngine;
using UnityEngine.AI;

public class Test_LD_Room : MonoBehaviour
{
    public Transform spawn;
    [SerializeField] private NavMeshSurface[] navMsS;

    private void OnEnable()
    {
        if (navMsS.Length > 0)
        {
            foreach (NavMeshSurface navMS in navMsS)
            {
                navMS.BuildNavMesh();
            }
        }
    }
}