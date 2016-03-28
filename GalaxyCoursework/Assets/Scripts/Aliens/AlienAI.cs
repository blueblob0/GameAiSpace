using UnityEngine;
using System.Collections;

/*
 * The class that will control the AI of the creatures
 */

public class AlienAI : MonoBehaviour {

    //Set in inspector
    public ushort speed = 1;
   
    // Use this for initialization
    public virtual void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}

    protected Vector3 seek(Vector3 targetWorldPos) {
        return Vector3.zero;
    }

    protected Vector3 flee(Vector3 targetWorldPos) {
        return Vector3.zero;
    }

    protected Vector3 wander() {
        return Vector3.zero;
    }
}
