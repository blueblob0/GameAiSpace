//Script made by: 626224
using UnityEngine;
using System.Collections;
using System;

/*
 * Checks if the reproduction target is in range
 */
public class ReproductionTargetInRange : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public ReproductionTargetInRange(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        return Vector3.Distance(agentRef.transform.position, agentRef.getReproductionTarget().transform.position) <= (5 / agentRef.getPlanetScale());
    }
}
