using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TestLink : MonoBehaviour
{
    NavMeshAgent agent;

    Transform playerTransfrom;

   /* [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;*/
    [SerializeField] Vector3 destination;

    //[SerializeField] bool switchPos;

    AgentLinkMover agentLinkMover;
    [SerializeField] NavMeshLink navLink;

    [SerializeField] float delayBeforeJump = 1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agentLinkMover = GetComponent<AgentLinkMover>();
        playerTransfrom = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        destination = playerTransfrom.position;
        agent.SetDestination(destination);

        if (agent.isOnOffMeshLink)
        {
            agentLinkMover.enabled = true;

            Invoke("EnabledLink", 1.9f);
            Debug.Log(agent.isOnOffMeshLink);
            navLink = agent.navMeshOwner as NavMeshLink;
            //agent.ActivateCurrentOffMeshLink(false);
            /* agentLinkMover.m_Curve.AddKey(0.5f, Mathf.Abs((navLink.endPoint.y - navLink.startPoint.y) / 1.5f));
             agentLinkMover._height = Mathf.Abs((navLink.endPoint.y - navLink.startPoint.y) / 1.5f);*/
            agentLinkMover.m_Curve.AddKey(0.5f, 10f);
            agentLinkMover._height = Mathf.Abs(10f);

            if (DelayBeforeJump() <= 0)
            {
                agentLinkMover._StopJump = false;
            }
        }
        else
        {
            agentLinkMover._StopJump = true;
            delayBeforeJump = 1f;
            navLink.UpdateLink();
        }
        /* if (Mouse.current.leftButton.wasPressedThisFrame)
         {
             agent.SetDestination(transform.parent.TransformPoint(switchPos ? pos1 : pos2));
             switchPos = !switchPos;
         }*/
    }

    void EnabledLink()
    {
        agent.ActivateCurrentOffMeshLink(true);
    }

    float DelayBeforeJump()
    {
        if (delayBeforeJump >0)
        {
            delayBeforeJump -= Time.deltaTime;
        }
        return delayBeforeJump;
    }
}