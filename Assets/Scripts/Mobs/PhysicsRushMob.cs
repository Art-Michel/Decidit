using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRushMob : MonoBehaviour
{
    CapsuleCollider capsuleCollider;
    private CharacterController controller;


    [Header("Gravity collision")]
    [SerializeField] LayerMask maskCollision;
    [SerializeField] List<Collider> listCol = new List<Collider>();
    [SerializeField] bool isFall;
    [SerializeField] bool isGround;
    [SerializeField] bool endMove;

    [Header("Gravity value")]
    [SerializeField] float gravity;
    [SerializeField] float fallingTime;


    [Header("Movement value")]
    [SerializeField] Transform target;
    private Vector3 playerVelocity;
    [SerializeField] float speedMove;
    [SerializeField] float velocity;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if(!endMove)
        {
            //CreateCollider();
            ApplyMovement();
        }

        if (!isFall && !isGround)
        {
            isFall = true;
        }
        else if(isFall && isGround)
        {
            endMove = true;
        }
    }

    void CreateCollider()
    {
        listCol.Clear();

        Vector3 direction = new Vector3 { [capsuleCollider.direction] = 1 };
        float offset = capsuleCollider.height / 2 - capsuleCollider.radius;
        Vector3 localPoint0 = capsuleCollider.center - direction * offset;
        Vector3 localPoint1 = capsuleCollider.center + direction * offset;

        Vector3 point0 = transform.TransformPoint(localPoint0);
        Vector3 point1 = transform.TransformPoint(localPoint1);

        Collider[] cols = Physics.OverlapCapsule(point0, point1, capsuleCollider.radius, maskCollision);
        listCol.AddRange(cols);

        if (listCol.Count > 0)
        {
            isGround = true;
            fallingTime = 1;
        }
        else
        {
            isGround = false;
        }
    }

    void ApplyMovement()
    {
        Vector2 targetPos = new Vector2(target.position.x, target.position.z);
        Vector2 direction = targetPos - (new Vector2(transform.position.x, transform.position.z));
        direction = direction.normalized * speedMove;

        SetGravity();

        Vector3 move = new Vector3(direction.x, playerVelocity.y, direction.y);
        controller.Move(move * Time.deltaTime);
        velocity = playerVelocity.y;
    }
    void SetGravity()
    {
        if (!isGround)
        {
            fallingTime += Time.deltaTime;
            float effectiveGravity = gravity * fallingTime;
            playerVelocity.y += effectiveGravity;
        }
        else
        {
            playerVelocity.y = 0;
        }
    }
}