//Script made by: 626224
using UnityEngine;
using System.Collections;
using System;

/*
 * The task to check if an enemy is target this agent
 */
public class EnemyTargetingSelf : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public EnemyTargetingSelf(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Loop through near targets
        foreach(AlienAI target in agentRef.getNearTargets()) {
            //If any of the near targets are targetting this agent
            if(target.getTarget() != null && target.getTarget() == agentRef) {
                //Set the target and return true
                agentRef.setTarget(target);
                return true;
            }
        }
        //False if none are targetting this
        return false;
    }
}
