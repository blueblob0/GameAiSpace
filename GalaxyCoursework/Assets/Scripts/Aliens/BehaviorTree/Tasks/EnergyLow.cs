//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * Task to check if the agent's energy is low
 */
public class EnergyLow : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public EnergyLow(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //True if energy is below 25%
        return agentRef.getEnergy() <= agentRef.getMaxEnergy() * 0.25;
    }
}
