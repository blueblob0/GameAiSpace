//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * The sequence to make the agent rest
 */
public class RestSequence : Sequence {
    //Tasks for this sequence
    private EnergyNotFull eNotFull;
    private Rest rest;

    //constructor
    public RestSequence(AlienAI agent) : base(agent) {
        //Construct the tasks
        eNotFull = new EnergyNotFull(agent);
        rest = new Rest(agent);

        //Add the children, order is important
        addChild(eNotFull);
        addChild(rest);
    }
}
