using UnityEngine;
using System.Collections;

/*
 * An alien creature with agressive characteristics
 *
 * The agent will seek out targets to attack
 * The agent will always fight back
 */
public class AgressiveAlien : AlienCreature {

    //Empty for now

    // Use this for initialization
    //void Start () {

    //}

    // Update is called once per frame
    public override void Update() {
        base.Update();


        /*
        basic procedure:
        check if any targets are seeking/attacking this agent
        respond
        check to attack any target
        flock with group
        */


        //DEBUG-----------------------------------------------------------------
        if(target == null) {
            if(getCreatureList().Count > 0) {
                //Compute the flocking
                addSteeringForce(computeFlocking(getCreatureList().ToArray()));
            }
            //Wander around
            addSteeringForce(wander());
            if(nearTargets.Count > 0) {
                target = nearTargets[0];
            }
        } else {
            addSteeringForce(persue(target));
        }
        //---------------------------------------------------------------------
    }
}
