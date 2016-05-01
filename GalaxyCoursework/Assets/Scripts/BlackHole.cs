using UnityEngine;
using System.Collections;

public class BlackHole : CelestialBody
{
    public int count =  20;
    public bool move = false;
    bool check = false;
	// Use this for initialization
	protected override void  Start ()
    {
        base.Start();
        mass = 500; 
        transform.localScale = Vector3.one * (mass );

        if (!controler)
        {
            controler = FindObjectOfType<CreateGalaxy>();
        }
        int holdcount = count; //store count as it would be lowered by any stars spawning sindie the hole 
        controler.RemoveStarsInsideBH();
        count = holdcount;
    }

    


	
	// Update is called once per frame
	void Update () {
        if (move)
        {
            UpdateTowardsBlackHole();

        }

    }
   

    public void UpdateTowardsBlackHole()
    {

        if (count > 0)
        {
            controler.moveStars(transform.position, mass);
            controler.RemoveStarsInsideBH();
        }        

    }



    void OnTriggerEnter(Collider other)
    {
       // Debug.Log("1");
        if(other.tag != "CelestialBody")
        {
            return;
        }
      
         //getting rid of for now as it dosent wrok on start 
        
        
        //mass -= other.GetComponent<CelestialBody>().Mass;
        //going to try removing count isntead of mass as big stars isntatly remove the black hole 
        //count--;

       // count -= Mathf.CeilToInt(Mathf.Log10(other.GetComponent<CelestialBody>().mass));

       // controler.DestroyStar(other.gameObject);
        
    }


}
