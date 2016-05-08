//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * Task checks if there are allies that can reproduce then sets it as the target
 */
public class AlliesReadyToReproduce : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public AlliesReadyToReproduce(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        if(agentRef.getOtherCreatures().Count <= 0) {
            return false;
        } else {
            foreach(GameObject alien in agentRef.getOtherCreatures()) {
                if(alien.GetComponent<AlienAI>().canReproduce()) {
                    agentRef.setReproductionTarget(alien.GetComponent<AlienAI>());
                    return true;
                }
            }
            return false;
        }
    }
}
