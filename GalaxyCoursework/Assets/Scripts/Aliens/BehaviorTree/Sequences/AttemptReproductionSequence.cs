using UnityEngine;
using System.Collections;

/*
 * Checks if the target is in range, then produces
 */
public class AttemptReproductionSequence : Sequence {
    //Tasks for this sequence
    private ReproductionTargetInRange inRange;
    private Reproduce reproduce;

    //constructor
    public AttemptReproductionSequence(AlienAI agent) : base(agent) {
        //Construct the tasks
        inRange = new ReproductionTargetInRange(agent);
        reproduce = new Reproduce(agent);

        //Add the children, order is important
        addChild(inRange);
        addChild(reproduce);
    }
}
