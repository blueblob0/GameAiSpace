using UnityEngine;
using System.Collections;

/*
 * An alien creature with passive characteristics
 *
 * The agent will not seek out targets to attack
 * The agent will almost never fight back
 */
public class PassiveAlien : AlienCreature {

    //Empty for now

	//// Use this for initialization
	//void Start () {
	
	//}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();

        if(getCreatureList().Count > 0) {
            //Compute the flocking
            addSteeringForce(computeFlocking(getCreatureList().ToArray()));
        }
        //Wander around
        addSteeringForce(wander());
    }
}
