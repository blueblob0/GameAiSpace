//Script made by: 626224
using UnityEngine;
using System.Collections;
using System;

/*
 * Task to check if an agent's energy is not full
 */
public class EnergyNotFull : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public EnergyNotFull(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        return agentRef.getEnergy() < agentRef.getMaxEnergy();
    }
}
