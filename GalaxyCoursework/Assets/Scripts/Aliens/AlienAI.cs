using UnityEngine;
using System.Collections;

/*
 * The class that will control the AI of the creatures
 */

public class AlienAI : MonoBehaviour {

    //Set in inspector
    public ushort speed = 1;

    //The velocity vector
    private Vector3 velocity;
   
    // Use this for initialization
    public virtual void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
        //Update the agent's position based on the velcoity
        transform.position += (velocity * speed * Time.deltaTime);

	}

    /// <summary>
    /// Makes the agent seek the target
    /// </summary>
    /// <param name="currentPos">Current position of the agent</param>
    /// <param name="targetWorldPos">The world position of the target to seek</param>
    /// <returns></returns>
    protected Vector3 seek(Vector3 currentPos, Vector3 targetWorldPos) {
        Vector3 ret = targetWorldPos - currentPos;
        ret.Normalize();

        return ret;
    }

    /// <summary>
    /// Makes the agent flee from the target
    /// </summary>
    /// <param name="currentPos">Current position of the agent</param>
    /// <param name="targetWorldPos">World position of the target to flee from</param>
    /// <returns></returns>
    protected Vector3 flee(Vector3 currentPos, Vector3 targetWorldPos) {
        Vector3 ret = currentPos - targetWorldPos;
        ret.Normalize();

        return ret;
    }

    protected Vector3 wander() {
        return Vector3.zero;
    }
}
