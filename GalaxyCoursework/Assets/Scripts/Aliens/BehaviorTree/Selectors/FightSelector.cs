using UnityEngine;
using System.Collections;

/*
 * Selector for the fight behavior
 */
public class FightSelector : Selector {
    //Tasks for this selector
    private EngageTargetSequence engage;
    private GetTargetSequence getTargSq;

    //constructor
    public FightSelector(AlienAI agent) : base (agent) {
        //Construct the tasks
        engage = new EngageTargetSequence(agent);
        getTargSq = new GetTargetSequence(agent);

        //Add the children, order is important
        addChild(engage);
        addChild(getTargSq);
    }
}
