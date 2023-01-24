using UnityEngine;
using UnityEngine.AI;

namespace State.WallAI
{
    public class GlobalRefWallAI : MonoBehaviour
    {
        [Header("References")]
        public Transform areaWallAI;
        public NavMeshAgent agent;
        public Animator animator;
        public Transform spawnBullet;
        public EnemyHealth enemyHealth;
        public BoxCollider[] walls;
        public Transform playerTransform;
        public float orientation;
        public MeshRenderer meshRenderer;
        public AudioSource audioSource;

        [Header("Scripatble")]
        public BaseMoveWallAISO baseMoveWallAISO;
        public BaseAttackWallAISO baseAttackWallAISO;

        // Start is called before the first frame update
        void Awake()
        {
            baseMoveWallAISO = Instantiate(baseMoveWallAISO);
            baseAttackWallAISO = Instantiate(baseAttackWallAISO);
            baseMoveWallAISO.lastWallCrack = Instantiate(baseMoveWallAISO.wallCrackPrefab, transform.position, Quaternion.Euler(0, orientation, 0));

            playerTransform = GameObject.FindWithTag("Player").transform;

            for (int i = 0; i < areaWallAI.childCount; i++)
            {
                walls[i] = areaWallAI.GetChild(i).GetComponent<BoxCollider>();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("WallAI"))
            {
                orientation = other.transform.localEulerAngles.y - 90 + 180;
            }
        }
    }
}