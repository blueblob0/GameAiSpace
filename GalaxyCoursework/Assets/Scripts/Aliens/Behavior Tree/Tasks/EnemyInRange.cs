﻿using UnityEngine;
using System.Collections;
using System;

public class EnemyInRange : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public EnemyInRange(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Returns true if the targets are in range
        return Vector3.Distance(agentRef.getTarget().transform.position, agentRef.transform.position) < 5;
    }
}
