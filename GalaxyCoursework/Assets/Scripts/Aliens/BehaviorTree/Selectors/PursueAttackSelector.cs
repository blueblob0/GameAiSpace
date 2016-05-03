using UnityEngine;
using System.Collections;

/*
 * Selector to pursue or attack agents
 */
public class PursueAttackSelector : Selector {
    //Tasks for this selector
    private AttackTargetSequence attack;
    private Pursue pursue;

    //constructor
    public PursueAttackSelector(AlienAI agent) : base(agent) {
        //Construct the tasks
        attack = new AttackTargetSequence(agent);
        pursue = new Pursue(agent);

        //Add the tasks, order matters
        addChild(attack);
        addChild(pursue);
    }
}
