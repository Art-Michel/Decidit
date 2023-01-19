using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRushMob : MonoBehaviour
{
    CapsuleCollider capsuleCollider;

    [Header("Gravity collision")]
    [SerializeField] LayerMask maskCollision;
    [SerializeField] List<Collider> listCol = new List<Collider>();
    [SerializeField] bool isFall;
    [SerializeField] bool isGround;

    [Header("Gravity value")]
    [SerializeField] float gravity;
    [SerializeField] float fallingTime;
    [SerializeField] float currentAcceleration;


    [Header("Movement value")]
    [SerializeField] Transform target;
    [SerializeField] float speedMove;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        CreateCollider();
        ApplyGravity();
        Movement();
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

    void ApplyGravity()
    {
        if(!isFall && !isGround)
        {
            isFall = true;
        }

        if(!isGround)
        {
            fallingTime += Time.deltaTime;
            float effectiveGravity = gravity * (fallingTime * fallingTime);
            currentAcceleration = effectiveGravity;
            transform.Translate(-Vector3.up * effectiveGravity * Time.deltaTime);
        }
    }

    void Movement()
    {
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z); 
        Vector3 direction = targetPos - transform.position;
        transform.Translate(direction * speedMove * Time.deltaTime);
    }
}