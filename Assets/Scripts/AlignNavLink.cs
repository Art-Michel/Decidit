using NaughtyAttributes;

namespace UnityEngine.AI
{
    public class AlignNavLink : MonoBehaviour
    {
        [SerializeField] bool aligneToX;
        [SerializeField] bool aligneToZ;
        NavMeshLink navMeshLink;

        [Button]
        public void AligneAxe()
        {
            navMeshLink = GetComponent<NavMeshLink>();

            if (aligneToX)
                navMeshLink.endPoint = new Vector3(navMeshLink.startPoint.x, navMeshLink.endPoint.y, navMeshLink.endPoint.z);

            if (aligneToZ)
                navMeshLink.endPoint = new Vector3(navMeshLink.endPoint.x, navMeshLink.endPoint.y, navMeshLink.startPoint.z);
        }
    }
}
