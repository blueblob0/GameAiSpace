//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * Selector for recouperating the agent
 */
public class RecoupSelector : Selector {
    //Tasks for this selector
    private RestSequence rest;
    private HealSequence heal;

    //constructor
    public RecoupSelector(AlienAI agent) : base(agent) {
        //Construct the tasks
        rest = new RestSequence(agent);
        heal = new HealSequence(agent);

        //Add the children, order is not as important
        addChild(rest);
        addChild(heal);
    }
}
