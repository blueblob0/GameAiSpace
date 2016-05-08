//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * Task to check if an agent's health is not full
 */
public class HealthNotFull : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public HealthNotFull(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        return agentRef.getHealth() < agentRef.getMaxHealth();
    }
}
