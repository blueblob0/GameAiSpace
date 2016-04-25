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

        //Perform an action based on the current state
        //switch(currentState) {
        //    case State.Wandering:
        //        if(getCreatureList().Count > 0) {
        //            //Compute the flocking
        //            addSteeringForce(computeFlocking(getCreatureList().ToArray()));
        //        }
        //        //Wander around
        //        addSteeringForce(wander());
        //        if(nearTargets.Count > 0) {
        //            target = nearTargets[0];
        //        }
        //        break;

        //    case State.Persuing:
        //        //... nothing for now
        //        break;

        //    case State.Fleeing:
        //        //... nothing for now
        //        break;

        //    case State.Attacking:
        //        //... nothing for now
        //        break;
        //}

        //Check the stats on nearby targets to find one to kill
        if(target == null) {
            foreach(AlienCreature creature in nearTargets) {
                if(shouldAttack(creature) > -2) {
                    target = creature;
                }
            }
        } else {
            if(Vector3.Distance(target.transform.position, transform.position) > 3) {
                currentState = State.Persuing;
            } else {
                currentState = State.Attacking;
            }
        }


        /*
        basic procedure:
        check if any targets are seeking/attacking this agent
        respond
        check to attack any target
        flock with group
        */


        //DEBUG-----------------------------------------------------------------
        //if(target == null) {
        //    if(getCreatureList().Count > 0) {
        //        //Compute the flocking
        //        addSteeringForce(computeFlocking(getCreatureList().ToArray()));
        //    }
        //    //Wander around
        //    addSteeringForce(wander());
        //    if(nearTargets.Count > 0) {
        //        target = nearTargets[0];
        //    }
        //} else {
        //    if(Vector3.Distance(transform.position, target.transform.position) > 2) {
        //        addSteeringForce(persue(target));
        //    } else {
        //        stopMovement();
        //        shouldAttack(target);
        //        //damageCreature(target);
        //    }
        //}
        //---------------------------------------------------------------------
    }
}
