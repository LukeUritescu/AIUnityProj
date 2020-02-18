using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Transition { None = 0, SawPlayer, ReachPlayer, LostPlayer, NoHealth}

public enum FSMStateID { None = 0, Patrolling, Chasing, Attacking, Dead}

public class AdvancedFSM : FSM
{
    private List<FSMState> fsmStates;

    private FSMStateID currentStateID;
    public FSMStateID CurrrentStateID
    {
        get
        {
            return currentStateID;
        }
    }

    private FSMState currentState;
    public FSMState CurrentState
    {
        get
        {
            return currentState;
        }
    }

    public AdvancedFSM()
    {
        fsmStates = new List<FSMState>();
    }

    public void AddFSMState(FSMState fsmState)
    {
        if(fsmState == null)
        {
            Debug.LogError("FSM ERROR:  Null reference is not allowed");
        }

        if(fsmStates.Count == 0)
        {
            fsmStates.Add(fsmState);
            currentState = fsmState;
            currentStateID = fsmState.ID;
            return;
        }

        foreach(FSMState state in fsmStates)
        {
            if(state.ID == fsmState.ID)
            {
                Debug.LogError("FSM ERROR: Tryinng to add a state that was already inside the list");
                return;
            }
        }

        fsmStates.Add(fsmState);
    }

    public void DeleteState(FSMStateID fsmState)
    {
        if(fsmState  == FSMStateID.None)
        {
            Debug.LogError("FSM ERROR: bull id is  not allowed");
            return;
        }
        foreach (FSMState state in fsmStates)
        {
            if(state.ID == fsmState)
            {
                fsmStates.Remove(state);
                return;
            }
        }
        Debug.Log("FSM ERROR: the state passed was not on the list. Impossible to delete it");
    }

    public void PerformTransition(Transition trans)
    {
        //Check for nulltransition
        if(trans == Transition.None)
        {
            Debug.LogError("FSM ERROR: Null transition");
            return;
        }

        FSMStateID id = currentState.GetOutputState(trans);
        if(id == FSMStateID.None)
        {
            Debug.LogError("FSM ERROR: Cururent state does not have a target state");
            return;
        }
        currentStateID = id;
        foreach(FSMState state in fsmStates)
        {
            if(state.ID == currentStateID)
            {
                currentState = state;
                break;
            }
        }
    }
}
