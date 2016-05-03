using UnityEngine;
using System.Collections;

/*
 * Selector for the fight behavior
 */
public class FightSelector : Selector {
    //Tasks for this selector
    //private AttackSequence attackSq;
    //private PursueSequence pursueSq;
    private GetTargetSequence getTargSq;

    //constructor
    public FightSelector(AlienAI agent) : base (agent) {
        //Construct the tasks
        //attackSq = new AttackSequence(agent);
        //pursueSq = new PursueSequence(agent);
        getTargSq = new GetTargetSequence(agent);

        //Add the children, order is important
        //addChild(attackSq);
        //addChild(pursueSq);
        addChild(getTargSq);
    }
}
