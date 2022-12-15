using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAI : MonoBehaviour
{
    Transform playerTransform;
    SphereCollider colliderDetectWAll;
    RaycastHit hitRight;
    RaycastHit hitLeft;

    [Header("Attack Rate")]
    [SerializeField] float maxRateAttack;
    [SerializeField] float currentRateAttack;
    [SerializeField] float maxDistanceDetectPlayer;
    [SerializeField] bool searchPlayer;

    [Header("Speed Movement  ")]
    [SerializeField] float currentSpeed;
    [SerializeField] float baseSpeed;
    [SerializeField] float contournSpeed;
    [SerializeField] float chargeSpeed;
    [SerializeField] float stopSpeed;
    [SerializeField] bool isRotating;
    [SerializeField] Vector3 currentRotation;
    [SerializeField] Vector3 oldRotation;


    [Header("Speed Rotation Patrol")]
    [SerializeField] float maxSpeedRotationAIPatrol;
    [SerializeField] float speedRotationAIPatrol;
    [SerializeField] float smoothRotationPatrol;

    [Header("Speed Rotation Controurne Wall")]
    [SerializeField] float maxSpeedRotationContourneWall;
    [SerializeField] float speedRotationContourneWall;
    [SerializeField] float smoothRotationContourneWall;
    
    [Header("Speed Rotation Charge")]
    [SerializeField] float maxSpeedRotationCharge;
    [SerializeField] float smoothRotationCharge;
    [SerializeField] float speedRotationCharge;


    [Header("Destination Variable (Debug)")]
    [SerializeField] float distDestination;
    [SerializeField] bool newPosIsSet;
    [SerializeField] Vector3 destinationFinal;
    [SerializeField] BoxCollider myCollider;
    [SerializeField] LayerMask noMask;

    [Header("Obstacle Variable (Debug)")]
    [SerializeField] bool obstacleTrigger;
    [SerializeField] bool obstacleOnPathDetected;
    [SerializeField] bool checkObstacleIsOnPass;
    [SerializeField] bool contournDirChose;
    [SerializeField] float dirContourne;
    [SerializeField] Transform obstacleTransform;
    [SerializeField] Vector3 obstaclePos;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        colliderDetectWAll = GetComponent<SphereCollider>();
        currentRateAttack = maxRateAttack;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(checkObstacleIsOnPass || obstacleTrigger)
            Detection();
    }

    private void Update()
    {
        distDestination = Vector3.Distance(transform.position, destinationFinal);

        SpeedManager();
        ApplyFlyingMove();
        RateAttack();

        // si la position aléatoir de patroll est défini l'IA si dirige, sinon on en cherche une
        if (newPosIsSet)
        {
            if (!obstacleOnPathDetected)
            {
                SmoothLookAtIA();
            }
        }
        else
        {
            SearchNewPos();
        }

        // si un obstacle est détecté est que l'IA n'est pas assez loin on cherche la direction de la destination (droite ou gauche) pour savoir de quel coté contourné l'obstacle
        if (obstacleOnPathDetected)
        {
            speedRotationAIPatrol = 0;
            CheckDestinationDir();
        }
        else
        {
            dirContourne = 0;
            speedRotationContourneWall = 0;
        }

        if (Vector3.Distance(oldRotation, transform.eulerAngles) == 0)
        {
            isRotating = false;
            speedRotationAIPatrol = 0.01f;
        }
        else
        {
            oldRotation = transform.eulerAngles;
            isRotating = true;
        }
    }

    void SmoothLookAtIA() // Rotation de l IA
    {
        Vector3 relativePos;

        relativePos.x = destinationFinal.x - transform.position.x;
        relativePos.y = destinationFinal.y - transform.position.y;
        relativePos.z = destinationFinal.z - transform.position.z;

        if(!searchPlayer)
        {
            Quaternion rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), speedRotationAIPatrol);
            transform.rotation = rotation;

            if (speedRotationAIPatrol < maxSpeedRotationAIPatrol)
            {
                speedRotationAIPatrol += (Time.deltaTime / smoothRotationPatrol);
            }
            else
            {
                speedRotationAIPatrol = 0;
            }
        }
        else
        {
            Quaternion rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), speedRotationCharge);
            transform.rotation = rotation;

            if (speedRotationCharge < maxSpeedRotationCharge)
            {
                speedRotationCharge += Time.deltaTime / smoothRotationCharge;
            }
            else
            {
               // speedRotationCharge = 0;
            }
        }
    }

    ////////////// Set Destination \\\\\\\\\\\\\\\\\\\\\
    Vector3 SearchNewPos() // défini la position aléatoire choisi dans la fonction "RandomPointInBounds()" si la distance entre le point et l'IA est suffisament grande
    {
        destinationFinal = RandomPointInBounds(myCollider.bounds);

        if (distDestination < 20)
        {
            newPosIsSet = false;
            return destinationFinal;
        }
        else
        {
            newPosIsSet = true;
            return destinationFinal;
        }
    }
    Vector3 RandomPointInBounds(Bounds bounds) // renvoie une position aléatoire dans un trigger collider
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    ////////////// Contourne Obstacle \\\\\\\\\\\\\\\\\\\\\
    void CheckDestinationDir() // si la destination est a droite ou à gauche, lance la fonction "ContourneDirection()"
    {
        contournDirChose = true;

        if (dirContourne > 0)
        {
            ContourneDirection(Vector3.right);
        }
        else if(dirContourne <0)
        {
            ContourneDirection(Vector3.left);
        }
    }
    void ContourneDirection(Vector3 vectorDirection) // effectue une rotation vers la droite ou la gauche en fonction de la pos du point de destination par rapport a l'IA
    {
        Vector3 relativePos;
        relativePos.x = transform.TransformDirection(vectorDirection).x;
        relativePos.y = 0;
        relativePos.z = transform.TransformDirection(vectorDirection).z;

        if (!searchPlayer)
        {
            Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), speedRotationContourneWall);
            transform.rotation = rotation;

            if (speedRotationContourneWall < maxSpeedRotationContourneWall)
            {
                speedRotationContourneWall += (Time.deltaTime / smoothRotationContourneWall);
            }
            else
            {
                speedRotationContourneWall = 0;
            }
        }
        else
        {
            Quaternion rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), speedRotationCharge);
            transform.rotation = rotation;

            if (speedRotationCharge < maxSpeedRotationCharge)
            {
                speedRotationCharge += Time.deltaTime / smoothRotationCharge;
            }
            else
            {
                speedRotationCharge = 0;
            }
        }
    }

    void RateAttack()
    {
        Vector2 playerPo;
        playerPo.x = playerTransform.position.x;
        playerPo.y = playerTransform.position.z;

        Vector2 thisPo;
        thisPo.x = transform.position.x;
        thisPo.y = transform.position.z;

        Debug.LogWarning(Vector2.Distance(playerPo, thisPo));

        if (currentRateAttack > 0 && !searchPlayer)
        {
            currentRateAttack -= Time.deltaTime;
        }
        else if (currentRateAttack <= 0)
        {
            Vector2 playerPos;
            playerPos.x = playerTransform.position.x;
            playerPos.y = playerTransform.position.z;

            Vector2 thisPos;
            thisPos.x = transform.position.x;
            thisPos.y = transform.position.z;

            Debug.LogWarning(Vector2.Distance(playerPos, thisPos));

            if (Vector2.Distance(playerPos, thisPos) < maxDistanceDetectPlayer)
            {
                currentRateAttack = maxRateAttack;
                newPosIsSet = true;
                searchPlayer = true;
            }
        }

        if (searchPlayer)
        {
            destinationFinal.x = playerTransform.position.x;
            destinationFinal.y = playerTransform.position.y + 1;
            destinationFinal.z = playerTransform.position.z;

            speedRotationContourneWall = 0;
            speedRotationAIPatrol = 0;
            colliderDetectWAll.radius = 4f;
        }
        else
        {
            speedRotationCharge = 0;
            colliderDetectWAll.radius = 4f;
        }
    }

    // Speed Movement 
    void SpeedManager()
    {
        if(!searchPlayer)
        {
            if (obstacleOnPathDetected)
            {
                if (currentSpeed > contournSpeed)
                    currentSpeed -= Time.deltaTime * 10;
            }
            else
            {
                if (currentSpeed < baseSpeed)
                    currentSpeed += Time.deltaTime * 10;
                else if (currentSpeed > baseSpeed)
                    currentSpeed -= Time.deltaTime * 10f;
            }
        }
        else
        {
            if (obstacleOnPathDetected)
            {
                if (currentSpeed > contournSpeed)
                    currentSpeed -= Time.deltaTime * 50;
            }
            else
            {
                if (currentSpeed < chargeSpeed)
                    currentSpeed += Time.deltaTime * 10;
                else if (currentSpeed > chargeSpeed)
                    currentSpeed = chargeSpeed;
            }

            if (distDestination <= 1f)
            {
                if(currentSpeed > stopSpeed)
                    currentSpeed -= Time.deltaTime * 500;

                if (currentSpeed < stopSpeed)
                    currentSpeed = 0;
            }
            else if (distDestination > 2.5f && currentSpeed < chargeSpeed && !obstacleOnPathDetected)
                currentSpeed += Time.deltaTime * 10;
        }
    }

    // Movement 
    void ApplyFlyingMove()
    {
        if (newPosIsSet)
        {
            if(!searchPlayer)
            {
                if (distDestination > 7)
                {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.TransformDirection(Vector3.forward), currentSpeed * Time.deltaTime);
                }
                else
                    newPosIsSet = false;
            }
            else
            {
                if (distDestination > 1)
                {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.TransformDirection(Vector3.forward), currentSpeed * Time.deltaTime);
                }
                else
                {
                    searchPlayer = false;
                    newPosIsSet = false;
                }
            }
        }
    }

    void RayDetectObstacle()
    {
        //forward Right
        hitRight = RaycastAIManager.RaycastAI(this.transform.position + transform.right / 1f, transform.forward, noMask, Color.green, 4f);
        //forward Left
        hitLeft = RaycastAIManager.RaycastAI(this.transform.position + -transform.right / 1f, transform.forward, noMask, Color.green, 4f);
    }

    void Detection()
    {
        RayDetectObstacle();

        if (obstacleTransform.tag == "Obstacle")
        {
            RaycastHit hit = RaycastAIManager.RaycastAI(this.transform.position, transform.forward, noMask, Color.red, Vector3.Distance(transform.position, obstacleTransform.position));
            obstaclePos = hit.point;

            float angle;
            Vector3 direction = obstacleTransform.position - this.transform.position;
            angle = Vector3.SignedAngle(this.transform.TransformDirection(Vector3.forward), direction, Vector3.up);

            Debug.DrawRay(this.transform.position, obstacleTransform.position - this.transform.position, Color.blue);
            Debug.Log(angle);

            if (hitRight.transform != null && hitLeft.transform == null)
            {
                if (hitRight.transform.tag == "Obstacle") // l obstacle sur la droite 
                {
                    obstacleOnPathDetected = true;
                    dirContourne = -1; // contourne par la gauche
                }
            }
            else if (hitLeft.transform != null && hitRight.transform == null)
            {
                if (hitLeft.transform.tag == "Obstacle") // l obstacle sur la gauche
                {
                    obstacleOnPathDetected = true;
                    dirContourne = 1; // contourne par la droite
                }
            }
            else if (hitRight.transform != null && hitLeft.transform != null)
            {
                if (hitRight.transform.tag == "Obstacle" && hitLeft.transform.tag == "Obstacle") // l obstacle en face
                {
                    obstacleOnPathDetected = true;
                    if (angle > 0)
                    {
                        dirContourne = -1; // contourne par la gauche
                    }
                    else if (angle < 0)
                    {
                        dirContourne = 1; // contourne par la droite
                    }
                }
            }
            else
            {
                obstacleOnPathDetected = false;
                contournDirChose = false;
                checkObstacleIsOnPass = false;
            }
        }
        else if (obstacleTransform.tag == "Wall")
        {
            RaycastHit hit = RaycastAIManager.RaycastAI(this.transform.position, transform.forward, noMask, Color.red, colliderDetectWAll.radius);
            float angle;

            if (!obstacleOnPathDetected)
            {
                obstaclePos = hit.point;
                angle = Vector3.SignedAngle(this.transform.TransformDirection(Vector3.forward), hit.normal, Vector3.up);
                Debug.Log(angle);
                Debug.Log(hit.distance);

                if (hitRight.transform != null && hitLeft.transform == null)
                {
                    if (hitRight.transform.tag == "Wall")
                    {
                        obstacleOnPathDetected = true;
                        dirContourne = -1;
                    }
                }
                else if (hitLeft.transform != null && hitRight.transform == null)
                {
                    if (hitLeft.transform.tag == "Wall")
                    {
                        obstacleOnPathDetected = true;
                        dirContourne = 1;
                    }
                }
                else if (hitRight.transform != null && hitLeft.transform != null)
                {
                    if (hitRight.transform.tag == "Wall" && hitLeft.transform.tag == "Wall")
                    {
                        obstacleOnPathDetected = true;
                        if (angle > 0)
                        {
                            dirContourne = 1; // contourne par la gauche
                        }
                        else if (angle < 0)
                        {
                            dirContourne = -1; // contourne par la droite
                        }
                    }
                }
            }
            if (hitRight.transform == null && hitLeft.transform == null)
            {
                obstacleOnPathDetected = false;
                contournDirChose = false;
                checkObstacleIsOnPass = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Wall"))
        {
            contournDirChose = false;
            obstacleTrigger = true;
            checkObstacleIsOnPass = true;

            if (obstacleTransform != other.transform || obstacleTransform==null)
            {
                obstacleTransform = other.transform;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Wall"))
        {
            obstacleTrigger = true;
            checkObstacleIsOnPass = true;
            obstacleTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Wall"))
        {
            obstacleTrigger = false;
            contournDirChose = false;
        }
    }
}