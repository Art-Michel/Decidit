using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace State.AICAC
{
    public class SurroundManager : MonoBehaviour
    {
        NavMeshHit closestHit;

        [Header("List AI Surround")]
        [SerializeField] List<GlobalRefAICAC> aiCACScriptsList = new List<GlobalRefAICAC>();
       
        [Header("Angle Surround")]
        [SerializeField] float maxAngle;
        public float radius;
        [SerializeField] float currentAnglePlacement;

        [SerializeField] Vector3 destination;

        [SerializeField] LineRenderer circleRenderer;
        [SerializeField] bool drawLineRenderer;

        // Start is called before the first frame update
        void Start()
        {
            SetListActiveAI();
        }
        public void SetListActiveAI()
        {
            aiCACScriptsList.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                aiCACScriptsList.Add(transform.GetChild(i).GetComponent<GlobalRefAICAC>());
            }
        }

        // Update is called once per frame
        void Update()
        {
            SetupPositionEnemy();
        }

        private void FixedUpdate()
        {
            if (drawLineRenderer)
                DrawCicre((int)maxAngle, (int)radius);
        }

        void SetupPositionEnemy()
        {
            currentAnglePlacement = 0f;
            float angleStep = maxAngle / aiCACScriptsList.Count;
            Vector3 centerPosition = CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer;

            for (int i = 0; i <= aiCACScriptsList.Count-1; i++)
            {
                float unitDirXposition = centerPosition.x + Mathf.Sin((currentAnglePlacement * Mathf.PI) / 180) * radius;//radius;
                float unitDirZposition = centerPosition.z + Mathf.Cos((currentAnglePlacement * Mathf.PI) / 180) * radius;//radius;

                destination = CheckNavMeshPoint(new Vector3(unitDirXposition, centerPosition.y, unitDirZposition));
                aiCACScriptsList[i].destination = destination;
                currentAnglePlacement += angleStep;
            }
        }

        void DrawCicre(int steps, int radius)
        {
            circleRenderer.positionCount = steps;

            for(int currentStep =0; currentStep < steps; currentStep++)
            {
                float circumFrequenceProgress = ((float)currentStep / steps);

                float currentRadian = circumFrequenceProgress * 2 * Mathf.PI;

                float xScale = Mathf.Cos(currentRadian);
                float zScale = Mathf.Sin(currentRadian);

                float x = xScale * radius;
                float z = zScale * radius;

                Vector3 currentPosition = CheckNavMeshPoint(new Vector3(CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer.x + x, 
                                                                         CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer.y, 
                                                                         CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer.z + z));
                circleRenderer.SetPosition(currentStep, currentPosition);
            }
        }
        Vector3 CheckNavMeshPoint(Vector3 _destination)
        {
            if (NavMesh.SamplePosition(_destination, out closestHit, 20, 1))
            {
                _destination = closestHit.position;
            }
            return _destination;
        }
    }
}