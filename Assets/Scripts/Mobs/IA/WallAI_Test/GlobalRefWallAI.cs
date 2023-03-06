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
        public List<Collider> wallsList = new List<Collider>();
        public Transform playerTransform;
        public float orientation;
        public MeshRenderer meshRenderer;
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
            if(areaWallAI == null)
                areaWallAI = transform.parent.transform.Find("Area_WallAI").transform;

            for (int i = 0; i < areaWallAI.childCount; i++)
            {
                if(areaWallAI.GetChild(i).GetComponent<BoxCollider>() == null)
                {
                    wallsList.Add(areaWallAI.GetChild(i).GetComponent<MeshCollider>());
                }
                else
                {
                    wallsList.Add(areaWallAI.GetChild(i).GetComponent<BoxCollider>());
                }
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

        public void CheckHP()
        {
            if (enemyHealth._hp <= 0 && !isDead)
            {
                ActiveState(StateControllerWallAI.WallAIState.Death);
                isDead = true;
            }
        }
    }
}