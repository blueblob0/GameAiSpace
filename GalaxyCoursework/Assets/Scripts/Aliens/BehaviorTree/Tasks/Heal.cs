//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * The task to replish health
 */
public class Heal : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public Heal(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Stop movement
        agentRef.setTargetSpeed(0);
        //Increase energy
        agentRef.increaseHealth(0.1f);
        return true;
    }
}
