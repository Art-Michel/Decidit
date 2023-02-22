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
        public float maxRadius;
        public float radius;
        public float adjustRadius;
        public float offsetRadius;
        [SerializeField] float currentAnglePlacement;
        [SerializeField] float currentAnglePlacementAdjust;
        [SerializeField] float currentAnglePlacementAdjust2;

        [SerializeField] Vector3 destination;

        [SerializeField] LineRenderer circleRenderer;
        [SerializeField] bool drawLineRenderer;

        [SerializeField] List<float> posYcircle = new List<float>();

        // Start is called before the first frame update
        void Start()
        {
            SetListActiveAI();

            for (int i = 0; i < maxAngle; i++)
            {
                posYcircle.Add(0);
            }
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
                aiCACScriptsList[i].destinationSurround = destination;
                currentAnglePlacement += angleStep;
            }
            CheckYPos();
        }

        void CheckYPos()
        {
            currentAnglePlacementAdjust = 0f;
            Vector3 centerPosition = CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer;

            for (int i = 0; i < maxAngle; i++)
            {
                float unitDirXposition = centerPosition.x + Mathf.Sin((currentAnglePlacementAdjust * Mathf.PI) / 180) * adjustRadius;//radius;
                float unitDirZposition = centerPosition.z + Mathf.Cos((currentAnglePlacementAdjust * Mathf.PI) / 180) * adjustRadius;//radius;

                posYcircle[i] = CheckNavMeshPoint(new Vector3(unitDirXposition, centerPosition.y, unitDirZposition)).y;

                if (i > 0)
                {
                    if ((int)posYcircle[i] != (int)posYcircle[i - 1])
                    {
                        Debug.Log(i);
                        adjustRadius--;
                        radius = adjustRadius - offsetRadius;
                        return;
                    }
                }
                currentAnglePlacementAdjust += 1;
            }

            CheckNextPosCircle();
        }

        void CheckNextPosCircle()
        {
            Vector3 centerPosition = CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer;

            if (PositionAreSame())
            {
                float checkAdjustRadius = adjustRadius + 2;
                currentAnglePlacementAdjust2 = 0f;

                for (int i = 0; i < maxAngle; i++)
                {
                    //Debug.Log(checkAdjustRadius);

                    float unitDirXposition = centerPosition.x + Mathf.Sin((currentAnglePlacementAdjust2 * Mathf.PI) / 180) * checkAdjustRadius;//radius;
                    float unitDirZposition = centerPosition.z + Mathf.Cos((currentAnglePlacementAdjust2 * Mathf.PI) / 180) * checkAdjustRadius;//radius;

                    posYcircle[i] = CheckNavMeshPoint(new Vector3(unitDirXposition, centerPosition.y, unitDirZposition)).y;

                    if (i > 0)
                    {
                        if ((int)posYcircle[i] != (int)posYcircle[i - 1])
                        {
                            Debug.Log(i);
                            Debug.Log("Not Up Circle");
                            return;
                        }
                    }
                    currentAnglePlacementAdjust2 += 1;
                }
                if (adjustRadius < maxRadius)
                {
                    Debug.Log("Up Circle");
                    adjustRadius++;
                    radius = adjustRadius - offsetRadius;
                }
            }
        }

        bool PositionAreSame()
        {
            for (int i = 0; i < posYcircle.Count-1; i++)
            {
                if ((int)posYcircle[i] != (int)posYcircle[i + 1])
                {
                    return false;
                }
            }
            return true;
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
            if (NavMesh.SamplePosition(_destination, out closestHit, radius, 1))
            {
                _destination = closestHit.position;
            }
            return _destination;
        }
    }
}