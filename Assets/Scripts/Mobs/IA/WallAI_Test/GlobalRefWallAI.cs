using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using NaughtyAttributes;

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
        public LayerMask maskPlayer;

        [Header("Animation")]
        public Animator myAnimator;
        public AnimationClip attackAnim;
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

        [Foldout("VeryEasy")] public BaseMoveWallAISO baseMoveWallAISO_VeryEZ;
        [Foldout("VeryEasy")] public BaseAttackWallAISO baseAttackWallAISO_VeryEZ;

        [Foldout("Easy")] public BaseMoveWallAISO baseMoveWallAISO_EZ;
        [Foldout("Easy")] public BaseAttackWallAISO baseAttackWallAISO_EZ;

        [Foldout("Medium")] public BaseMoveWallAISO baseMoveWallAISO_Med;
        [Foldout("Medium")] public BaseAttackWallAISO baseAttackWallAISO_Med;

        [Foldout("Hard")] public BaseMoveWallAISO baseMoveWallAISO_Hard;
        [Foldout("Hard")] public BaseAttackWallAISO baseAttackWallAISO_Hard;

        [Foldout("VeryHard")] public BaseMoveWallAISO baseMoveWallAISO_VeryHard;
        [Foldout("VeryHard")] public BaseAttackWallAISO baseAttackWallAISO_VeryHard;

        // Start is called before the first frame update
        void Awake()
        {
            switch (ApplyDifficulty.indexDifficulty)
            {
                case 0:
                    baseMoveWallAISO = Instantiate(baseMoveWallAISO_VeryEZ);
                    baseAttackWallAISO = Instantiate(baseAttackWallAISO_VeryEZ);
                    break;

                case 1:
                    baseMoveWallAISO = Instantiate(baseMoveWallAISO_EZ);
                    baseAttackWallAISO = Instantiate(baseAttackWallAISO_EZ);
                    break;

                case 2:
                    baseMoveWallAISO = Instantiate(baseMoveWallAISO_Med);
                    baseAttackWallAISO = Instantiate(baseAttackWallAISO_Med);
                    break;

                case 3:
                    baseMoveWallAISO = Instantiate(baseMoveWallAISO_Hard);
                    baseAttackWallAISO = Instantiate(baseAttackWallAISO_Hard);
                    break;

                case 4:
                    baseMoveWallAISO = Instantiate(baseMoveWallAISO_VeryHard);
                    baseAttackWallAISO = Instantiate(baseAttackWallAISO_VeryHard);
                    break;
            }

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

        private void Update()
        {
            CheckHP();
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