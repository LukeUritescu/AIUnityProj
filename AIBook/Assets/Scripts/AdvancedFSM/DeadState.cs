﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : FSMState
{
    public DeadState()
    {
        stateID = FSMStateID.Dead;
    }

    public override void Reason(Transform player, Transform npc)
    {
        
    }

    public override void Act(Transform player, Transform npc)
    {
        
    }
}
