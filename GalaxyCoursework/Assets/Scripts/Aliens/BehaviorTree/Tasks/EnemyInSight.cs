//Script made by: 626224
using UnityEngine;
using System.Collections;
using System;

/*
 * The task to check if an enemy is in sight
 */
public class EnemyInSight : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public EnemyInSight(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Will be true if there are Enemies near
        return agentRef.getNearTargets().Count > 0;
    }
}
