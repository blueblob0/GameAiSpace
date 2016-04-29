using UnityEngine;
using System.Collections;

/*
 * Sequence to pursue targets
 */
public class PursueSequence : Sequence {
    //Tasks for this sequence
    private HaveTarget haveTarget;
    private Pursue pursue;

    //constructor
    public PursueSequence(AlienAI agent) : base (agent) {
        //Construct the tasks
        haveTarget = new HaveTarget(agent);
        pursue = new Pursue(agent);

        //Add the children, order is important
        addChild(haveTarget);
        addChild(pursue);
    }
}
