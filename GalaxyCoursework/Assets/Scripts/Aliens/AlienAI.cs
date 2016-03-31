using UnityEngine;
using System.Collections;

/*
 * The class that will control the AI of the creatures
 */

public class AlienAI : MonoBehaviour {

    //Set in inspector
    public ushort speed = 1;
    public ushort steeringForce = 5;

    //The target of the agent
    private GameObject target;

    //The velocity vector
    private Vector3 velocity;
    //Used to apply steering
    private Vector3 steering;
    private Vector3 desiredVelocity;

    //DEBUG
    private GameObject testCube;
    private float wait = 3;
    private float then = 0;

    // Use this for initialization
    public virtual void Start () {
        velocity = transform.forward * speed * Time.deltaTime;
        steering = Vector3.zero;
        desiredVelocity = Vector3.zero;

        testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testCube.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    }
	
	// Update is called once per frame
	public virtual void Update () {
        //DEBUG---------------------
        if(then >= wait) {
            testCube.transform.position = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));

            target = testCube;

            then = 0;
        } else {
            then += Time.deltaTime;
        }
        //--------------------------

        displayDebugLines();

        //Get a steering force
        if(target) {
            steering = seek(target.transform.position);
        }

        //Add to the velocity
        velocity += steering;

        //Normalise the desired velocity and add the speed
        velocity.Normalize();
        velocity *= speed * Time.deltaTime;

        //Update the position
        transform.position += velocity;
    }

    /// <summary>
    /// Makes the agent seek the target
    /// </summary>
    /// <param name="targetWorldPos">The world position of the target to seek</param>
    /// <returns>Returns the steering force</returns>
    protected Vector3 seek(Vector3 targetWorldPos) {
        //The steering force
        Vector3 force;

        //Set the diesried velocity to the direction of the target
        desiredVelocity = targetWorldPos - transform.position;
        //Scale to the speed of the agent
        desiredVelocity.Normalize();
        desiredVelocity *= speed * Time.deltaTime;

        //Return the steering force of the desired velocity
        force = desiredVelocity - velocity;
        return force;

    }

    /// <summary>
    /// Makes the agent flee from the target
    /// </summary>
    /// <param name="currentPos">Current position of the agent</param>
    /// <param name="targetWorldPos">World position of the target to flee from</param>
    /// <returns></returns>
    protected Vector3 flee(Vector3 currentPos, Vector3 targetWorldPos) {
        return currentPos - targetWorldPos;
    }

    /// <summary>
    /// Makes the agent wander around
    /// </summary>
    /// <param name="curentPos">Current position of the agent</param>
    /// <param name="agentForward">Agent's forward vector</param>
    /// <param name="wanderOffSet">How far forward to set the wander pos</param>
    /// <returns></returns>
    protected Vector3 wander(Vector3 curentPos, Vector3 agentForward, float wanderOffSet = 5.0f) {
        //Create the 'circle' for the velcoity to be in
        Vector3 circleCenter = curentPos + (agentForward * wanderOffSet);
        //The radius of the circle will be how far the displacement can go
        float circleRadius = wanderOffSet / 2;

        //Init the displacement force (direction to wander to)
        Vector3 displacement = new Vector3();
        //Get an angle anywhere from 0 - 360
        float angle = Random.Range(0, 360);
        //Displace the vector by the angle
        displacement.x = circleRadius * Mathf.Cos(angle);
        displacement.z = circleRadius * Mathf.Sin(angle);

        //Return the new wander force
        return circleCenter + displacement;
    }

    /// <summary>
    /// Displays the debug lines for the velocity(Green), steering(Red) and desired velocity(Blue)
    /// </summary>
    private void displayDebugLines() {
        //Desired velocity
        Debug.DrawLine(transform.position, transform.position + (desiredVelocity.normalized * 5), Color.blue);
        //Steering
        Debug.DrawLine(transform.position, transform.position + (steering.normalized * 5), Color.red);
        //Velocity
        Debug.DrawLine(transform.position, transform.position + (velocity.normalized * 5), Color.green);
    }
}
