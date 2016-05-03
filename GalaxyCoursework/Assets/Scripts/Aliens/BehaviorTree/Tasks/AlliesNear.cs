using UnityEngine;
using System.Collections;
using System;

/*
 * The task to check if allies are near
 */
public class AlliesNear : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public AlliesNear(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //returns true if there are other creatures near
        return agentRef.getOtherCreatures().Count > 0;
    }
}
