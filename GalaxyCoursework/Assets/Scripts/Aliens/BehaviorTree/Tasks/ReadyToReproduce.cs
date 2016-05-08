//Script made by: 626224
using UnityEngine;
using System.Collections;
using System;

/*
 * Task to check if the agent can reproduce
 */
public class ReadyToReproduce : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public ReadyToReproduce(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        return agentRef.canReproduce();
    }
}
