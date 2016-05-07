
using UnityEngine;
using System.Collections;

public class BlackHole : CelestialBody
{
    public  int count{ get; private set; }
   
//bool check = false;
	// Use this for initialization
	protected override void  Start ()
    {
        base.Start();       
        

        if (!controler)
        {
            controler = FindObjectOfType<CreateGalaxy>();
        }
        int holdcount = count; //store count as it would be lowered by any stars spawning sindie the hole 
        controler.RemoveStarsInsideBH(this);
        count = holdcount;
    }


    protected override void SetScale()
    {
        transform.localScale = Vector3.one * mass *CreateGalaxy.starMuti/50; 
    }

    /// <summary>
    /// sets the number of stars the black hole can take in before it stops
    /// </summary>
    /// <param name="isBig">if the black hole is the one in th center</param>
    public void SetCount(bool isBig)
    {
        if (isBig)
        {
            count = 20;
        }
        else
        {
            count = 1;
        }
    }

    public void reduceCount(int amount)
    {
        count -= amount;

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
