using UnityEngine;
using State.AICAC;

public class RaycastCrossHair : MonoBehaviour
{
    RaycastHit hit;
    [SerializeField] LayerMask mask;
    [SerializeField] Transform crossHair;
    [SerializeField] Transform playerT;
    [SerializeField] Camera cam;
    [SerializeField] int maxRangeChanceToActiveDodge;

    GlobalRefAICAC globalRef;

    // Start is called before the first frame update
    void Start()
    {
        playerT = GameObject.FindWithTag("Player").transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 startPos = cam.ScreenToWorldPoint(crossHair.position);

        hit = RaycastAIManager.RaycastAI(startPos, playerT.forward, mask, Color.black, 100f);

        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("Ennemi"))
            {
                if (hit.transform.GetComponent<GlobalRefAICAC>() != null)
                {
                    globalRef = hit.transform.GetComponent<GlobalRefAICAC>();

                    if (Random.Range(0, maxRangeChanceToActiveDodge) == 10)
                    {
                        float angle;
                        angle = Vector3.SignedAngle(playerT.forward, hit.normal, Vector3.up);

                        if (angle > 0)
                        {
                            globalRef.dodgeAICACSO.targetObjectToDodge = this.transform;
                            globalRef.dodgeAICACSO.rightDodge = true;
                            globalRef.ActiveStateDodge();
                        }
                        else
                        {
                            globalRef.dodgeAICACSO.targetObjectToDodge = this.transform;
                            globalRef.dodgeAICACSO.leftDodge = true;
                            globalRef.ActiveStateDodge();
                        }
                    }
                }
            }
        }
    }
}
