using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : FSMState
{
    public ChaseState(Transform[] wp)
    {
        wayPoints = wp;
        stateID = FSMStateID.Chasing;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

        //find next Waypoint position
        FindNextPoint();
    }

    public override void Reason(Transform player, Transform npc)
    {
        //set target position as  player pos
        destPos = player.position;
        //check  distance with player tank
        float dist = Vector3.Distance(npc.position, destPos);
        if(dist  <= 200.0f)
        {
            Debug.Log("Switch to Attack state");
            npc.GetComponentInChildren<NPCTankController>().SetTransition(Transition.ReachPlayer);

        }

        //Go back to patrol if too far
        else if (dist >= 300.0f)
        {
            Debug.Log("Switch to patrol state");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LostPlayer);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //Rotate  to the target point
        destPos = player.position;

        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //Go  Forward
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

}
