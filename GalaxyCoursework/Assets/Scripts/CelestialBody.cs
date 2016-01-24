using UnityEngine;
//using System;

public abstract class CelestialBody : MonoBehaviour {
    float totalGravityForce;
    public float G=10; //= 6.674f * Mathf.Pow(10, 11);
    public float speed;
    public float moveAmount;
    public double mass;
    // Use this for initialization
    protected abstract void Start();

    // Update is called once per frame
    protected abstract void Update(); 
    /// <summary>
    /// spawing Satalite around things 
    /// </summary>
    /// <param name="moveAmount">The distance you have to move to start spawning from</param>
    /// <param name="minDist"> The min dsitance from previous one</param>
    /// <param name="maxDist"> the max distance</param>
    /// <param name="prefabName"> name of prefab to spawn</param>
    /// <returns></returns>
    protected GameObject SpawnSatalite(float moveAmount, float minDist,float maxDist,string prefabName)
    {
        
        // star by moving out a bit from the planet 
        float move = moveAmount + Random.Range(minDist, maxDist);
        //Debug.Log(move);
        Vector3 starPos;

        starPos =   Random.insideUnitCircle.normalized * move;
        //Vector3 starPos = Vector3.one * move;
        starPos.z = starPos.y;
        starPos.y = 0;
        starPos += transform.position;
        //Debug.Log(MoveAmount + " " + move + " " + Vector3.Distance(Vector3.zero, starPos));
        GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
        Satalite holds = a.GetComponent<Satalite>();
        holds.orbitingBody = gameObject;
        holds.distPlanet = move;
         
        a.name = move.ToString();
        SetSataliteSpeed(holds);
        a.transform.SetParent(gameObject.transform);                
        a.transform.position = starPos;
        return a;
    }
    
    /// <summary>
    /// sets  the speed of a given satlite 
    /// </summary>
    /// <param name="sat"></param>
    /// <returns></returns>
  protected virtual void SetSataliteSpeed(Satalite sat)
    {
        // uses Orbital Speed Equation
        double aSpeed = 0;

        aSpeed = System.Math.Sqrt((G * System.Math.Sqrt(mass)) / sat.distPlanet);
    
        aSpeed /= Mathf.Pow(10, 17);

        aSpeed *= 60 * 60 * 24;
        //aSpeed = 30;
        sat.moveAmount = (2 * Mathf.PI * sat.distPlanet) / (float)aSpeed;
        sat.moveAmount = 360 / sat.moveAmount;

         Debug.Log(moveAmount + " " + sat.name);
        sat.speed = (float)aSpeed;
    }


   protected Vector3 RotatePoint(Vector3 pointToRotate, Vector3 centerPoint, double angleInDegrees)
    {
        double angleInRadians = angleInDegrees * (System.Math.PI / 180);
        double cosTheta = System.Math.Cos(angleInRadians);
        double sinTheta = System.Math.Sin(angleInRadians);
        return new Vector3
        {
            x =
                (int)
                (cosTheta * (pointToRotate.x - centerPoint.x) -
                sinTheta * (pointToRotate.z - centerPoint.z) + centerPoint.x),
            y = 0,
        z =
            (int)
            (sinTheta * (pointToRotate.x - centerPoint.x) +
            cosTheta * (pointToRotate.z - centerPoint.z) + centerPoint.z)
        };
    }



}

