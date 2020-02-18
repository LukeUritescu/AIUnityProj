using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMState
{
    protected Dictionary<Transition, FSMStateID> map = new Dictionary<Transition, FSMStateID>();
    protected FSMStateID stateID;
    public FSMStateID ID { get { return stateID; } }
    protected Vector3 destPos;
    protected Transform[] wayPoints;
    protected float curRotSpeed;
    protected float curSpeed;

    public void AddTransition(Transition transition, FSMStateID id)
    {
        if(transition == Transition.None || id == FSMStateID.None)
        {
            Debug.LogWarning("FSMState : Null transition not allowed");
            return;
        }

        if (map.ContainsKey(transition))
        {
            Debug.LogWarning("FSMState ERROR: transition is alrerady inside the map");
            return;
        }
        map.Add(transition, id);
        Debug.Log("Added: " + transition + " with ID: " + id);

    }

    public void DeleteTransition(Transition trans)
    {
        if(trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: Nulltransition is not allowed");
            return;
        }

        if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition Passed was not on this State's List");
    }

    public FSMStateID GetOutputState (Transition trans)
    {
        if(trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: Null transition is not allowed");
            return FSMStateID.None;
        }
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        Debug.LogError("FSMState ERRORO " + trans + " Transition passeed to the State was not on the list");
        return FSMStateID.None;
    }

    public abstract void Reason(Transform player, Transform npc);
    public abstract void Act(Transform player, Transform npc);

    public void FindNextPoint()
    {
        int rndIndex = Random.Range(0, wayPoints.Length);
        Vector3 rndPosition = Vector3.zero;
        destPos = wayPoints[rndIndex].position + rndPosition;
    }


    protected bool IsInCurrentRange(Transform trans, Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - trans.position.x);
        float zPos = Mathf.Abs(pos.z - trans.position.z);

        if(xPos <= 50  && zPos <= 50)
        {
            return true;
        }

        return false;
    }
}
