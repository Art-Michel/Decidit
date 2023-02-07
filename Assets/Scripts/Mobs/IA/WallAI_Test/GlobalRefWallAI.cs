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
        public AudioSource audioSourceWallMob;

        [Header("Animation")]
        public Animator myAnimator;
        public GlobalRefAnimator globalRefAnimator;

        [Header("Slow Move References")]
        public bool isInEylau;
        public float slowSpeed;
        public float slowRatio;

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
            audioSourceWallMob = GetComponentInChildren<AudioSource>();

            for (int i = 0; i < areaWallAI.childCount; i++)
            {
                wallsList.Add(areaWallAI.GetChild(i).GetComponent<BoxCollider>());
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