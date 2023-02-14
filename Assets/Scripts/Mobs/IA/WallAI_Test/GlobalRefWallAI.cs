using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace State.WallAI
{
    public class GlobalRefWallAI : MonoBehaviour
    {
        [Header("References")]
        public Transform areaWallAI;
        public NavMeshAgent agent;
        public Transform spawnBullet;
        public EnemyHealth enemyHealth;
        public List<BoxCollider> wallsList = new List<BoxCollider>();
        public Transform playerTransform;
        public float orientation;
        public MeshRenderer meshRenderer;
        //public AudioSource audioSourceWallMob;
        [SerializeField] StateControllerWallAI stateControllerWallAI;

        [Header("Animation")]
        public Animator myAnimator;
        public GlobalRefAnimator globalRefAnimator;

        [Header("Slow Move References")]
        public bool isInEylau;
        public float slowSpeed;
        public float slowRatio;

        [Header("Death")]
        public bool isDead;

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
            //audioSourceWallMob = GetComponentInChildren<AudioSource>();
            areaWallAI = transform.parent.transform.Find("Area_WallAI").transform;

            for (int i = 0; i < areaWallAI.childCount; i++)
            {
                wallsList.Add(areaWallAI.GetChild(i).GetComponent<BoxCollider>());
            }
        }

        private void Update()
        {
            if(enemyHealth._hp <=0 && !isDead)
            {
                ActiveState(StateControllerWallAI.WallAIState.Death);
                isDead = true;
            }
        }

        public void ActiveState(StateControllerWallAI.WallAIState newState)
        {
            stateControllerWallAI.SetActiveState(newState);
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