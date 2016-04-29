using UnityEngine;
using System.Collections;

/*
 * The sequence to make the agent rest
 */
public class RestSequence : Sequence {
    //Tasks for this sequence
    private EnergyLow eLow;
    private Rest rest;

    //constructor
    public RestSequence(AlienAI agent) : base(agent) {
        //Construct the tasks
        eLow = new EnergyLow(agent);
        rest = new Rest(agent);

        //Add the children, order is important
        addChild(eLow);
        addChild(rest);
    }
}
