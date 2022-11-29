using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IACAC : MonoBehaviour
{
    [Header("Variable speed movement")]
    [SerializeField] float currentSpeed;
    [SerializeField] float stopSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float dodgeSpeed;
    [SerializeField] float chargeSpeed;
    [SerializeField] float speedRotation;


    [Header("Variable distance to player")]
    [SerializeField] float distPlayer;

    NavMeshAgent agent;

    Transform playerTransform, currentTargetAgent, targetTransform, spawnRayDodge, spawnRayFollowPlayer, spawnRayInterPos, SpawnRayCharge, detectEnnemi;

    Vector3 targetDodgeVector, destinationNavMesh;

    [Header("point intermediaire entre la destination et l'IA")]
    [SerializeField] Vector3 pointIntermediaire;

    [Header("Variable detect if AI can dodge")]
    public bool canDodge;
    [SerializeField] bool isDodging;
    [SerializeField] bool rightDodge;
    [SerializeField] bool leftDodge;

    [Header("Variable detect when dodge is aviable")]
    [SerializeField] float dodgeCoolDown;
    [SerializeField] float currentDodgeCoolDown;

    [Header("Variable manage charge")]
    [SerializeField] float distCharge;
    [SerializeField] float currentChargeDuration;
    [SerializeField] float maxChargeDuration;
    [SerializeField] float delayBeforeCharge;
    [SerializeField] bool chargeDone;
    [SerializeField] bool activeChargeDelay;
    [SerializeField] bool isCharging;

    RaycastHit dodgeHit, chargeHit;
    Ray dodgeRay, chargeRay;
    NavMeshHit navHit;
    [SerializeField] LayerMask noMask, maskWalls, maskRayCharge;

    [Header("Variable bypass player")]
    [SerializeField] bool activeBypassPlayer;
    [SerializeField] bool isBypassingPlayer;
    [SerializeField] bool rightBypassPlayer;
    [SerializeField] float baseRotationRayInterPos;
    [SerializeField] bool activeSwitchBypass;
    [SerializeField] float rotationYSpawnRayInterPos;

    public bool giveEnnemipass;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        playerTransform = GameObject.FindWithTag("Player").transform;
        targetTransform = GameObject.FindWithTag("Player").transform;
        spawnRayDodge = transform.GetChild(0);
        spawnRayFollowPlayer = transform.GetChild(1);
        spawnRayInterPos = spawnRayFollowPlayer.GetChild(0);
        SpawnRayCharge = transform.GetChild(2);
        detectEnnemi = transform.GetChild(3);

        currentDodgeCoolDown = dodgeCoolDown;

        baseRotationRayInterPos = 41f;

        currentChargeDuration = maxChargeDuration;
    }

    void Update()
    {
        distPlayer = Vector3.Distance(transform.position, targetTransform.position);

        MoveToTarget();
        SmoothLookAt();

        SetCoolDownDodge();

        if (isDodging && !canDodge)
            Dodge();
        
        if(!ChargeIsActive())
            CalculateInterPos();

        agent.speed = SpeedManager();

        CheckCanCharge();

        TimingCharge();

        ActiveCharge();

        if (!canDodge && giveEnnemipass && !isDodging && !ChargeIsActive())
            canDodge = true;

        Debug.Log(DestinationNavMesh());
    }

    public void MoveToTarget()
    {
        if(canDodge)
        {
            SetAIDodgePos();
        }
        else
        {
            if(!isDodging)
            {
                agent.SetDestination(DestinationNavMesh());
            }
        }
        Debug.DrawRay(spawnRayDodge.position, spawnRayDodge.TransformDirection(Vector3.forward) * 5, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.blue);
    }

    // renvoie le vecteur utilisé comme point de destination de l agent
    Vector3 DestinationNavMesh()
    {
        return pointIntermediaire;
    }

    void SmoothLookAt() // Rotation de l IA
    {
        Vector3 direction;
        Vector3 relativePos;

        if (isBypassingPlayer || isCharging) // si l IA contourne le joueur ou si elle charge le joueur la rotation ce fait vers la destination de l agent
        {
            direction = agent.destination;
        }
        else // sinon la rotation ce fait vers le joueur
        {
            direction = playerTransform.position;
        }

        relativePos.x = direction.x - transform.position.x;
        relativePos.y = 0;
        relativePos.z = direction.z - transform.position.z;

        Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), speedRotation);
        transform.rotation = rotation;
    }

    // fonction qui renvoie vrai si le point "dodgePoint" se trouve sur le NavMesh et faux si ce n est pas le cas
    bool DetectDodgePointIsOnNavMesh(Vector3 dodgePoint)
    {
        if (NavMesh.SamplePosition(dodgePoint, out navHit, 0.1f, NavMesh.AllAreas))
        {
            Debug.Log("point on nav mesh");
            return true;
        }
        else
        {
            Debug.Log("point out nav mesh");
            return false;
        }
    }

    // verifie si les condition sont rempli pour faire une esquive
    public void CheckCanDodge()
    {
        if (Random.Range(0, 20) == 0 && !canDodge && !isDodging && currentDodgeCoolDown <= 0)
        {
            if (!ChargeIsActive() && !isBypassingPlayer)
                canDodge = true;
        }
    }

    // choisi la position de l esquive
    void SetAIDodgePos()
    {
        isDodging = true;

        if(Random.Range(0,2) ==0) // choisi l esquive par la droite 
        {
            rightDodge = true;
            SelectDodgeDirection();
        }
        else // choisi l esquive par la gauche 
        {
            leftDodge = true;
            SelectDodgeDirection();
        }

        canDodge = false;
    }

    void SelectDodgeDirection()
    {
        if (rightDodge) // choisi l esquive par la droite 
        {
            spawnRayDodge.localEulerAngles = new Vector3(spawnRayDodge.eulerAngles.x, (int)Random.Range(40, 90), 0);

           /* dodgeRay = new Ray(spawnRayDodge.position, spawnRayDodge.forward * 5);
            Physics.Raycast(dodgeRay, out dodgeHit);
            Debug.DrawRay(spawnRayDodge.position, spawnRayDodge.TransformDirection(Vector3.forward) * 5, Color.red);*/

            dodgeHit = RaycastAIManager.RaycastAI(spawnRayDodge.position, spawnRayDodge.forward, noMask, Color.red, 5f);

            if (!DetectDodgePointIsOnNavMesh(dodgeHit.point))
            {
                if(!leftDodge)
                {
                    leftDodge = true;
                }
                else
                {
                    isDodging = false;
                    canDodge = false;
                }
            }
            else
            {
                targetDodgeVector = dodgeHit.point;
            }
        }
        if(leftDodge) // choisi l esquive par la gauche 
        {
            spawnRayDodge.localEulerAngles = new Vector3(spawnRayDodge.eulerAngles.x, (int)Random.Range(-40, -90), 0);

            dodgeRay = new Ray(spawnRayDodge.position, spawnRayDodge.forward * 5);
            Physics.Raycast(dodgeRay, out dodgeHit);
            Debug.DrawRay(spawnRayDodge.position, spawnRayDodge.TransformDirection(Vector3.forward) * 5, Color.red);

            if (!DetectDodgePointIsOnNavMesh(dodgeHit.point))
            {
                if (!rightDodge)
                {
                    rightDodge = true;
                }
                else
                {
                    isDodging = false;
                    canDodge = false;
                }
            }
            else
            {
                targetDodgeVector = dodgeHit.point;
            }
        }
    }

    // fonction qui effectue l esquive
    void Dodge()
    {
        if (DetectDodgePointIsOnNavMesh(targetDodgeVector)) // si le point de destination l esquive est pas sur le nav mesh
        {
            if (Vector3.Distance(transform.position, targetDodgeVector) > 1.1f)
            {
                isDodging = true;
                agent.SetDestination(targetDodgeVector);
            }
            else
            {
                isDodging = false;
                Debug.Log("Stop dodge");
            }
        }
        else // si le point de destination l esquive n est pas sur le nav mesh, l esquive est annulé
        {
            isDodging = false;
            canDodge = false;
        }
    }

    // Delay entre chaque esquive
    void SetCoolDownDodge()
    {
        if(currentDodgeCoolDown >0)
        {
            currentDodgeCoolDown -= Time.deltaTime;
        }
        else if(isDodging)
        {
            currentDodgeCoolDown = Random.Range(1,4);
        }
    }

    // défini un point intermediaire entre le joueur est l ia pour contourné le joueur
    Vector3 CalculateInterPos()
    {
        spawnRayFollowPlayer.LookAt(playerTransform); // GameObject vide qui suit le joueur du regard

        RaycastHit hit = RaycastAIManager.RaycastAI(spawnRayInterPos.position, spawnRayInterPos.TransformDirection(Vector3.forward), maskWalls, Color.green, 20f);
       /* Ray ray;

        ray = new Ray(spawnRayInterPos.position, spawnRayInterPos.TransformDirection(Vector3.forward)); // tire un raycast décalé de spawnRayInterPos.rotation.y° par rapport au joueur
        Physics.Raycast(ray, out hit, 20f, maskWalls); // le point renvoyé par le raycast est utilisé comme chemin pour contourné le joueur*/

        Debug.Log(hit.transform);

        if(activeBypassPlayer) // si l ia contourne le joueur
        {
            if (distPlayer > distCharge) // si l ia n est pas a distance de charge
            {
                if (hit.transform != null) 
                {
                    if (hit.transform.CompareTag("Ground"))
                    {
                        isBypassingPlayer = true;
                        pointIntermediaire = hit.point;
                    }
                }
                else
                {
                    activeSwitchBypass = true; // si aucun objet n est detecter l ia change de coté pour contourné le joueur
                }
            }
            else
            {
                isBypassingPlayer = false;
                pointIntermediaire = playerTransform.position;
            }
        }
        else
        {
            isBypassingPlayer = false;
            pointIntermediaire = playerTransform.position;
        }

        if (activeBypassPlayer)
            ActiveSwitchByPass();

        //Debug.DrawRay(spawnRayInterPos.position, spawnRayInterPos.TransformDirection(Vector3.forward) * 20, Color.green);
        return pointIntermediaire;
    }

    void ActiveSwitchByPass() 
    {
        float rotationSpawnRayInterPos = 0;
        if (spawnRayInterPos.localEulerAngles.y <= 180f)
        {
            rotationSpawnRayInterPos = spawnRayInterPos.localEulerAngles.y;
        }
        else
        {
            rotationSpawnRayInterPos = spawnRayInterPos.localEulerAngles.y - 360f;
        }
        if (activeSwitchBypass)
        {
            if (rightBypassPlayer && rotationSpawnRayInterPos > -baseRotationRayInterPos)
            {
                SwitchAroundPath();
            }
            if (!rightBypassPlayer && rotationSpawnRayInterPos < baseRotationRayInterPos)
            {
                SwitchAroundPath();
            }
        }
    }

    // change le coté par lequel l ia contourne le joueur (droite ou gauche)
    void SwitchAroundPath()
    {
        Debug.Log("dzdzdzdz");

        float Rotation;
        if (spawnRayInterPos.localEulerAngles.y <= 180f)
        {
            Rotation = spawnRayInterPos.localEulerAngles.y;
        }
        else
        {
            Rotation = spawnRayInterPos.localEulerAngles.y - 360f;
        }

        if (rightBypassPlayer && Rotation > -baseRotationRayInterPos)
        {
            if (Rotation <= -40)
            {
                rightBypassPlayer = false;
                activeSwitchBypass = false;
            }

            rotationYSpawnRayInterPos = 50 * Time.deltaTime;
            spawnRayInterPos.localRotation = Quaternion.Euler(spawnRayInterPos.localEulerAngles.x, Rotation - rotationYSpawnRayInterPos, spawnRayInterPos.localEulerAngles.z);
        }
        if (!rightBypassPlayer && Rotation < baseRotationRayInterPos)
        {
            if (Rotation >= 40)
            {
                rightBypassPlayer = true;
                activeSwitchBypass = false;
            }

            rotationYSpawnRayInterPos = 50 * Time.deltaTime;
            spawnRayInterPos.localRotation = Quaternion.Euler(spawnRayInterPos.localEulerAngles.x, Rotation + rotationYSpawnRayInterPos, spawnRayInterPos.localEulerAngles.z);
        }
    }

    void CheckCanCharge() // vérifie si l ia est a bonne distance et si il n est pas en train de charger pour relancer une charge 
    {
        if (chargeDone && distPlayer >= distCharge)
        {
            chargeDone = false;
        }
    }

    void ActiveCharge() // vérifie si l ia est a bonne distance et si il n est pas en train de charger pour relancer une charge 
    {
        if (distPlayer <= distCharge && !chargeDone && !isCharging)
        {
            if (currentChargeDuration > 0 && ChargeFoundDestination() && !activeChargeDelay && delayBeforeCharge > 0)
            {
                activeChargeDelay = true;
            }
            else
            {
                chargeDone = true;
                currentChargeDuration = maxChargeDuration;
            }
        }
    }

    void TimingCharge()
    {
        if (activeChargeDelay) // delay avant d effectuer la charge
        {
            if (delayBeforeCharge > 0)
            {
                delayBeforeCharge -= Time.deltaTime;
            }
            else
            {
                activeChargeDelay = false;
            }
        }
        else if (delayBeforeCharge <= 0) // defini le point de destination et la durée de la charge
        {
            currentChargeDuration -= Time.deltaTime;
            pointIntermediaire = chargeHit.point;
            isCharging = true;

            if (currentChargeDuration <= 0) // fin de la charge
            {
                delayBeforeCharge = 0.5f;
                currentChargeDuration = maxChargeDuration;
                chargeDone = true;
                isCharging = false;
            }
            else if(!ChargeFoundDestination())
            {
                delayBeforeCharge = 0.5f;
                currentChargeDuration = maxChargeDuration;
                chargeDone = true;
                isCharging = false;
            }
        }
    }

    bool ChargeFoundDestination() // tire un raycast en face de l ia
    {
        chargeHit = RaycastAIManager.RaycastAI(SpawnRayCharge.position, SpawnRayCharge.TransformDirection(Vector3.forward), maskRayCharge, Color.cyan, 5f);

        /*chargeRay = new Ray(SpawnRayCharge.position, SpawnRayCharge.TransformDirection(Vector3.forward));
        Physics.Raycast(chargeRay, out chargeHit, 5f, maskRayCharge);

        Debug.DrawRay(SpawnRayCharge.position, SpawnRayCharge.TransformDirection(Vector3.forward) * 5, Color.cyan);*/

        if (chargeHit.transform != null)
        {
            if (DetectDodgePointIsOnNavMesh(chargeHit.point) && chargeHit.transform.CompareTag("Ground"))
            {
                return true;
            }
            return false;
        }
        return false;
    }

    bool ChargeIsActive() // renvoie vrai si l IA commence une charge, renvoie faux si ce n est pas le cas
    {
        if (activeChargeDelay || isCharging)
            return true;
        else 
            return false;
    }

    float SpeedManager()
    {
        if (distPlayer <= 2)
            currentSpeed = stopSpeed;
        else if (isDodging)
            currentSpeed = dodgeSpeed;
        else if (isBypassingPlayer)
            currentSpeed = runSpeed;
        else if(activeChargeDelay)
            currentSpeed = stopSpeed;
        else if (isCharging)
            currentSpeed = chargeSpeed;
        else 
            currentSpeed = walkSpeed;

        return currentSpeed;
    }

    [SerializeField] List<GameObject> ennemiInCollider = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ennemi"))
        {
            if (ChargeIsActive())
            {
                if (!ennemiInCollider.Contains(other.gameObject) || ennemiInCollider == null)
                    ennemiInCollider.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ennemi"))
        {
            if(ChargeIsActive())
            {
                if (ennemiInCollider != null)
                {
                    for (int i = 0; i < ennemiInCollider.Count; i++)
                    {
                        Debug.Log("dzdzdzdz");

                        if (RaycastAIManager.RaycastAI(transform.position, ennemiInCollider[i].transform.position - transform.position, noMask, Color.yellow, 10f).transform != null)
                        {
                            ennemiInCollider[i].GetComponent<IACAC>().giveEnnemipass = true;
                        }

                        //Debug.DrawRay(transform.position, ennemiInCollider[i].transform.position - transform.position, Color.yellow);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ennemi"))
        {
            other.gameObject.GetComponent<IACAC>().giveEnnemipass = false;
            ennemiInCollider.Remove(other.gameObject);
        }
    }
}