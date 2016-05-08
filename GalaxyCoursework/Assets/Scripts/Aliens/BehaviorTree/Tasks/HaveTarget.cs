//Script made by: 626224
using UnityEngine;
using System.Collections;
using System;

/*
 * The task to check if the agent has a target
 */
public class HaveTarget : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public HaveTarget(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //True if the target is not null
        return agentRef.getTarget() != null;
    }
}
