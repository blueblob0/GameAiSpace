using UnityEngine;
using System.Collections;
using System;

/*
 * The task to check if this agent's health is low
 */
public class HealthLow : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public HealthLow(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //True if health is below 30%
        return agentRef.getHealth() <= agentRef.getMaxHealth() * 0.3;
    }
}
