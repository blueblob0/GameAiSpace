using UnityEngine;
using System.Collections;
using System;

/*
 * The task to replish energy
 */
public class Rest : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public Rest(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Stop movement
        agentRef.setTargetSpeed(0);
        //Increase energy
        agentRef.increaseEnergy(0.5f);
        return true;
    }
}
