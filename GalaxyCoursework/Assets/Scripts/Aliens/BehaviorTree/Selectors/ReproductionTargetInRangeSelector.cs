using UnityEngine;
using System.Collections;

/*
 * Selects the right task if the reproduction target is in range or not
 */
public class ReproductionTargetInRangeSelector : Selector {
    //Tasks for this selector
    private AttemptReproductionSequence attempt;
    private MoveToReproductionTarget move;

    //constructor
    public ReproductionTargetInRangeSelector(AlienAI agent) : base(agent) {
        //Construct the tasks
        attempt = new AttemptReproductionSequence(agent);
        move = new MoveToReproductionTarget(agent);

        //Add the tasks, order is important
        addChild(attempt);
        addChild(move);
    }
}
