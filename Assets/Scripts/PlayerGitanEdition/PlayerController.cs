using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Transform cam;
    [SerializeField] Transform spawnRay;
    [SerializeField] LayerMask mask;
    RaycastHit hit;

    [Header("Rotation")]
    [SerializeField] float rotSpeed;
    [SerializeField] float rotX;
    [SerializeField] float rotY;


    public CharacterController controller;

    [Header("Mouvement")]
    public Vector3 move = Vector3.zero;
    public bool groundedPlayer;
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = 9.81f;
    public bool isMooving;
    public float localVelocity;

    [Header("Mouvement")]
    public static int hpStatic;
    public int hp;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        cam = transform.GetChild(0);
        hpStatic = hp;
    }
    void Update()
    {
        hp = hpStatic;

        rotX -= rotSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
        rotY += rotSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0, rotY, 0);

        rotX = Mathf.Clamp(rotX ,-65f, 65f);
        cam.localRotation = Quaternion.Euler(rotX, 0, 0);

        groundedPlayer = controller.isGrounded;

        Deplacement();
    }

    private void FixedUpdate()
    {
        hit = RaycastAIManager.RaycastAI(spawnRay.position, transform.forward, mask, Color.black, 100f);

        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("Ennemi"))
            {
                if (hit.transform.GetComponent<StateManagerAICAC>() != null)
                {
                    if (Random.Range(0, 25) == 10)
                    {
                        RaycastHit hit = RaycastAIManager.RaycastAI(transform.position, transform.forward, mask, Color.red, 100f);
                        float angle;
                        angle = Vector3.SignedAngle(transform.forward, hit.normal, Vector3.up);

                        if (angle > 0)
                        {
                            hit.transform.GetComponent<StateManagerAICAC>().dodgeParameterAICACSOInstance.targetObjectToDodge = this.transform;
                            hit.transform.GetComponent<StateManagerAICAC>().dodgeParameterAICACSOInstance.rightDodge = true;
                            hit.transform.GetComponent<StateManagerAICAC>().SwitchToNewState(2);
                        }
                        else
                        {
                            hit.transform.GetComponent<StateManagerAICAC>().dodgeParameterAICACSOInstance.targetObjectToDodge = this.transform;
                            hit.transform.GetComponent<StateManagerAICAC>().dodgeParameterAICACSOInstance.leftDodge = true;
                            hit.transform.GetComponent<StateManagerAICAC>().SwitchToNewState(2);
                        }
                    }
                }
            }
        }
    }

    void Deplacement()
    {
        localVelocity = controller.velocity.magnitude;

        Vector2 dirXZ = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        move.x = dirXZ.x;
        move.z = dirXZ.y;

        //movement
        if (controller.isGrounded)
        {
            move.y = 0;
            if (Input.GetButtonDown("Jump"))
            {
                move.y += jumpHeight;
            }
            move = transform.TransformDirection(move);
        }
        else
            move = transform.TransformDirection(move);

        move.y += gravityValue * Time.deltaTime;
        controller.Move(move * playerSpeed *Time.deltaTime);

        if (controller.velocity != Vector3.zero)
            isMooving = true;
        else
            isMooving = false;

        RaycastAIManager.RaycastAI(transform.position, controller.velocity.normalized, mask, Color.red, Vector3.Distance(transform.position, controller.velocity.normalized));
    }

    public static void ApplyDamage(int damage)
    {
        hpStatic -= damage;
    }
}