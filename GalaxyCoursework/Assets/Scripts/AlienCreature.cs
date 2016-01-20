using UnityEngine;
using System.Collections;

/* The super class that will handle all the characteristics
 * of the alien creatures that will spawn, all the random spawning variables
 * like arms, legs, heads will happen here too
 */

public class AlienCreature : MonoBehaviour {

    //Reference to the creature's body
    private GameObject body;

    /// <summary>
    /// Will spawn the creature into the game world
    /// </summary>
    protected virtual void Start () {
        body = createBody();
        body.transform.SetParent(transform);
	}
	
	/// <summary>
    /// Update the creature to peform movement ect.
    /// </summary>
	protected virtual void Update () {
	
	}

    /// <summary>
    /// Spawns a body for the creature 
    /// </summary>
    /// <param name="maxHeads">The maximum amount of hezads the creature can have</param>
    /// <param name="maxArms">The maximum amount of arms the creature can have</param>
    /// <param name="maxLegs">The maximum amount of legs the creature can have</param>
    /// <returns>Returns a reference to the body created</returns>
    protected GameObject createBody(ushort maxHeads = 1, ushort maxArms = 2, ushort maxLegs = 2){
        GameObject returnObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //Set the name
        returnObject.name = "Body";

        //Return the new body
        return returnObject;
    }
}
