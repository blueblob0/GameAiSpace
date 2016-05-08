//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * The main sequence for reproduction
 */
public class ReproduceSequence : Sequence {
    //Tasks for this sequence
    private ReadyToReproduce ready;
    private AllyReproductionReadySequence allyReady;

    //constructor
    public ReproduceSequence(AlienAI agent) : base(agent) {
        //Construct the tasks in this sequences
        ready = new ReadyToReproduce(agent);
        allyReady = new AllyReproductionReadySequence(agent);

        //Add children, order is important
        addChild(ready);
        addChild(allyReady);
    }
}
