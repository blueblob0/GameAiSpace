using UnityEngine;
using System.Collections;

/*
 * Sequence for checking if the agent is ready
 */
public class ReadyToReproduceSequence : Sequence {
    //Tasks for this sequence
    private ReadyToReproduce ready;
    private AllyReproductionReadySequence allyReady;

    //constructor
    public ReadyToReproduceSequence(AlienAI agent) : base(agent) {
        //Construct the tasks in this sequences
        ready = new ReadyToReproduce(agent);
        allyReady = new AllyReproductionReadySequence(agent);

        //Add children, order is important
        addChild(ready);
        addChild(allyReady);
    }
}
