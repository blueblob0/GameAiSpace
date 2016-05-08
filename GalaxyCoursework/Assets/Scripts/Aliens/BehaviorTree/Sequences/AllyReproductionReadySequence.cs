//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * Sequence for checking if the ally is ready
 */
public class AllyReproductionReadySequence : Sequence {
    //Tasks for this sequence
    private AlliesReadyToReproduce ready;
    private ReproductionTargetInRangeSelector inRange;

    //constructor
    public AllyReproductionReadySequence(AlienAI agent) : base(agent) {
        //Construct the tasks in this sequences
        ready = new AlliesReadyToReproduce(agent);
        inRange = new ReproductionTargetInRangeSelector(agent);

        //Add children, order is important
        addChild(ready);
        addChild(inRange);
    }
}
