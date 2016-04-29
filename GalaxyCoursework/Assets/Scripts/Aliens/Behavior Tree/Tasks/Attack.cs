using UnityEngine;
using System.Collections;

/*
 * Task to attack enemys
 */
public class Attack : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public Attack(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Stop movement
        agentRef.setTargetSpeed(0);
        //Damage the creature then return
        agentRef.damageCreature(agentRef.getTarget());
        return true;
    }
}
