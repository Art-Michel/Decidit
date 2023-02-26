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
        float currentAnglePlacement;
        float currentAnglePlacementAdjust;
        float currentAnglePlacementAdjust2;
        Vector3 destination;

        [SerializeField] LineRenderer circleRenderer;
        [SerializeField] bool drawLineRenderer;

        List<Vector3> posCircle = new List<Vector3>();

        void Start()
        {
            SetListActiveAI();

            for (int i = 0; i < maxAngle; i++)
            {
                posCircle.Add(Vector3.zero);
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

        void Update()
        {
            CheckYPos();
            CheckNextPosCircle();
            if (PositionAreSame(posCircle))
                SetupPositionEnemy();
            else
            {
                adjustRadius--;
            }
        }

        private void FixedUpdate()
        {
            if (drawLineRenderer)
                DrawCicre((int)maxAngle, (int)radius);
        }

        void SetupPositionEnemy()
        {
            radius = adjustRadius - offsetRadius;
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
        }

        void CheckYPos()
        {
            currentAnglePlacementAdjust = 0f;
            Vector3 centerPosition = CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer;

            for (int i = 0; i < maxAngle; i++)
            {
                float unitDirXposition = centerPosition.x + Mathf.Sin((currentAnglePlacementAdjust * Mathf.PI) / 180) * adjustRadius;//radius;
                float unitDirZposition = centerPosition.z + Mathf.Cos((currentAnglePlacementAdjust * Mathf.PI) / 180) * adjustRadius;//radius;

                posCircle[i] = CheckNavMeshPoint(new Vector3(unitDirXposition, centerPosition.y, unitDirZposition));

                if (i > 0)
                {
                    if ((int)posCircle[i].y != (int)posCircle[i - 1].y)
                    {
                        adjustRadius--;
                        return;
                    }
                }
                currentAnglePlacementAdjust += 1;
            }
        }

        void CheckNextPosCircle()
        {
            Vector3 centerPosition = CheckPlayerDownPos.instanceCheckPlayerPos.positionPlayer;

            if (PositionAreSame(posCircle))
            {
                float checkAdjustRadius = adjustRadius;
                checkAdjustRadius += 1;
                currentAnglePlacementAdjust2 = 0f;

                for (int i = 0; i < maxAngle; i++)
                {
                    float unitDirXposition = centerPosition.x + Mathf.Sin((currentAnglePlacementAdjust2 * Mathf.PI) / 180) * checkAdjustRadius;//radius;
                    float unitDirZposition = centerPosition.z + Mathf.Cos((currentAnglePlacementAdjust2 * Mathf.PI) / 180) * checkAdjustRadius;//radius;

                    posCircle[i] = CheckNavMeshPoint(new Vector3(unitDirXposition, centerPosition.y, unitDirZposition));

                    if (i > 0)
                    {
                        if (posCircle[i].x == posCircle[i - 1].x)
                        {
                            return;
                        }
                        else if (posCircle[i].z == posCircle[i - 1].z)
                        {
                            return;
                        }
                        else if ((int)posCircle[i].y != (int)posCircle[i - 1].y)
                        {
                            return;
                        }
                    }
                    currentAnglePlacementAdjust2 += 1;

                    if (adjustRadius < maxRadius && i == maxAngle-1)
                    {
                        adjustRadius++;
                    }
                }
            }
        }

        bool PositionAreSame(List<Vector3> posYcircle)
        {
            for (int i = 0; i < posYcircle.Count-1; i++)
            {
                if ((int)posYcircle[i].y != (int)posYcircle[i + 1].y)
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
            if (NavMesh.SamplePosition(_destination, out closestHit, radius*2, 1))
            {
                _destination = closestHit.position;
            }
            return _destination;
        }
    }
}