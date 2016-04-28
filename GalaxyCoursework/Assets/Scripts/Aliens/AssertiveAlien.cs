using UnityEngine;
using System.Collections;

/*
 * An Alien creature with assertvie characteristics
 *
 * The agent will sometimes seek out targets to attack
 * The agent will fight back or flee
 */
public class AssertiveAlien : AlienCreature {

    //Empty for now

    //// Use this for initialization
    //void Start () {

    //}

    // Update is called once per frame
    public override void Update() {
        base.Update();

        //DEBUG-----------------------------------------------------------------
        if(getCreatureList().Count > 0) {
            //Compute the flocking
            //addSteeringForce(computeFlocking(getCreatureList().ToArray()));
        }
        //Wander around
        //addSteeringForce(wander());
        //----------------------------------------------------------------------
    }
}
