//script made by: up651590
using UnityEngine;
using System.Collections.Generic;

//Class to hold the star and move it before we spawn it
public class HoldStar
{
    public Vector3 starPos;
    private int starMass;
    public int StarMass
    {
        get{ return starMass; }
    }   

    public readonly int id;

    public float radius()
    {
        return (starMass * CreateGalaxy.starMuti)/2;
    }

    public HoldStar(int StarMass, int Id, Vector3 StarPos)
    {       
        id = Id;
        starPos = StarPos;
        starMass = StarMass;
    }

    public void IncreaseMass( int addMass)
    {
        starMass += addMass;
    }
}

public class CreateGalaxy : MonoBehaviour
{
    public const int starMuti = 50;
    public const int planetMuti = 50;
    //public int numStars = 100;
    private List<HoldStar> holdStars = new List<HoldStar>();
    private List<Star> realStars = new List<Star>();
    public const string starPrefabName = "StarPrefab";
    public const string binaryPrefabName = "BinaryStarPrefab";
    // private int spaceBetween = StarMuti*10;
    /// <summary>
    /// the center of the Galaxy might be usefull if you have mutiple galaxys 
    /// </summary>
    private Vector3 centerpos = Vector3.zero;
    //bool testing = false;
    bool backhole = true;
    //bool backhole = false;
    BlackHole black;

    //The way geenration will work
    //create stars
    //create balckhole
    //move the stars towards blackhole 
    //stop all stars
    //work out type of star from size
    //then work out planets around star
    //planets that can have life marked 

     //extra
     //make stars just struts holding the location and mass and move based on that  

    // Use this for initialization
    void Start()
    {        
        Application.runInBackground = true;
        GenerateGalaxy();
    }

    void GenerateGalaxy()
    {
        int theID = 0;
        int yPos = 0;
        int baseStarMass = 10;
        int radiusMax = 50;
        float radiusIncrease = 4 * (starMuti);
        int angelMax = 360;
        int spirals = 2;
        int sprialWidth = 50;
        float[] spiralAngel = new float[spirals];
        float angelhold = angelMax / spirals;
        for (int i = 0; i < spiralAngel.Length; i++)
        {
            spiralAngel[i] = i * angelhold;
        }

        //start by creating stars

        //This is how many "rows" of planets are in the spiral, (each row are a circle 
        for (float row = 1; row < radiusMax; row++)
        {
            // the current angle around the row 
            for (float angle = 0; angle < angelMax; angle += 10)
            {
                //then run for each arm of the spiral
                for (int i = 0; i < spiralAngel.Length; i++)
                {
                    // if the current angle is within the spiral then create some stars
                    if ((angle >= spiralAngel[i] && angle < spiralAngel[i] + sprialWidth || angle + 360 - sprialWidth <= spiralAngel[i] && angle + 360 > spiralAngel[i]))
                    {
                        float holdx = centerpos.x + (radiusIncrease * row) * Mathf.Cos(angle * Mathf.Deg2Rad);
                        float holdz = centerpos.z + (radiusIncrease * row) * Mathf.Sin(angle * Mathf.Deg2Rad);

                        float sample = Random.Range(0.5f, 0.9f); // The size of the star to generate                        
                        
                        HoldStar newStar = new HoldStar(Mathf.CeilToInt(sample * baseStarMass), theID, new Vector3(holdx * 10, yPos, holdz * 10));
                                       
                        theID++;
                        holdStars.Add(newStar);
                     
                        i = spiralAngel.Length;
                    }
                }
            }
            for (int i = 0; i < spiralAngel.Length; i++)
            {
                spiralAngel[i] += radiusIncrease / (starMuti); // This moves the angle that the star can spawn at along after each rotation
            }

        }
        //backhole = false;
        //once all the stars are made create a blackhole at the center 
        if (backhole)
        {
            GameObject ablack = Instantiate(Resources.Load("BlackHole")) as GameObject;
            ablack.transform.SetParent(transform);
            black = ablack.GetComponent<BlackHole>();
        }
        //then move the stars towards the blackhole
        MoveTowardsBlackHole();

        CatogriseStars();
       // Debug.Log(black.count);
    }


    private void CatogriseStars()
    {
        //types of star: tbd            (array pos)
        //BlackHole Star Over 50 Mass     (0)
        //Neutron Over 40 Mass           (1)
        //otherwise:
        //Singe Star 70%                (2)
        //Twin Star 20%                 (3)
        //Ternary star10%               (4)

        //int[] starCount = new int[5] { 0, 0, 0, 0, 0 };

        foreach (HoldStar s in holdStars)
        {
            if (s.StarMass > 50)
            {
                MakeStar(s, starType.BlackHole);                
            }
            else if(s.StarMass > 40)
            {
                MakeStar(s, starType.Neutron);
            }
            else
            {
                int holdRand = Random.Range(0,100);
                if (holdRand < 70)
                {
                    MakeStar(s, starType.SingeStar);
                }
                else if(holdRand < 90)
                {
                    MakeStar(s, starType.BinaryStar);
                }
                else 
                {
                    //Debug.Log("starType.Ternarystar");
                    MakeStar(s, starType.Ternarystar);
                }
            }            
        }        
    }

    /// <summary>
    /// Function to spawn a given type of star
    /// </summary>
    /// <param name="starInfo"> a Class holding the stars position and mass</param>
    /// <param name="sType"> The Type of star to spawn</param>
    private void MakeStar(HoldStar starInfo, starType sType)
    {
        GameObject stara;
        if (sType == starType.BinaryStar)
        {

             stara = (GameObject)Instantiate(Resources.Load(binaryPrefabName));
        }
        else
        {

            stara = (GameObject)Instantiate(Resources.Load(starPrefabName));
        }
        
       
        stara.transform.SetParent(transform);
        stara.transform.position = starInfo.starPos;
        Star theStar = stara.GetComponent<Star>();
        
        theStar.typeOfStar = sType;
        theStar.SetMass(starInfo.StarMass);
        
        theStar.AssignVarables();
        realStars.Add(theStar);
        if (sType == starType.Neutron)
        {
            
            stara.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            stara.name = "Neutron";
            Debug.Log("Neutron");
        }
        else if (sType == starType.BlackHole)
        {
            stara.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            stara.name = "BlackHole";
        }
        else if (sType == starType.SingeStar)
        {
            stara.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            stara.name = "SingleStar";
        }
        else if (sType == starType.BinaryStar)
        {
          for(int i =0;i< theStar.theRend.Length; i++)
            {
                theStar.theRend[i].material.SetColor("_Color", Color.yellow);
                
                
            }
            
            stara.name = "BinaryStar";
        }
        else if (sType == starType.Ternarystar)
        {
            for (int i = 0; i < theStar.theRend.Length; i++)
            {
                theStar.theRend[i].material.SetColor("_Color", Color.magenta);
                
            }
            stara.name = "Ternarystar";
        }
    }    
    
    //move every star towards the backhole
    public void MoveTowardsBlackHole()
    {
        int i = 0;
        while (black.count > 0 && i < 10000)
        {
            moveStars(black.transform.position, black.mass);
            RemoveStarsInsideBH(); //check for stars inside and destroy
            i++;
        }

    }
    /// <summary>
    /// make sure we remove planets inside the balckhole 
    /// </summary>
    public void RemoveStarsInsideBH()
    {
       // SphereCollider blackColl = black.GetComponent<SphereCollider>();

        HoldStar[] tempArray = holdStars.ToArray();

        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.lossyScale.x);

        for (int i = 0; i < tempArray.Length; i++)
        {
            float dis = Vector3.Distance(black.transform.position, tempArray[i].starPos); // Get Distance Between two stars 

            if (dis < ((black.mass/2) + tempArray[i].radius()))
            {
                //remove from the list so we dont try and acces a bull gameobjecct 
                //going to try removing count isntead of mass as big stars isntatly remove the black hole 
                //count--;
                black.count -= Mathf.CeilToInt(Mathf.Log10(tempArray[i].StarMass));
                holdStars.Remove(tempArray[i]);

            }           
            
        }
    }

    /// <summary>
    /// USed for when stars collide during the start time as oncollision does not work here 
    /// </summary>
    public void RemoveStarsInStar(HoldStar theStar)
    {
        HoldStar[] tempArray = holdStars.ToArray();

        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.lossyScale.x);
        bool restartCheck = false;
        for (int i = 0; i < tempArray.Length; i++)
        {
            float dis = Vector3.Distance(theStar.starPos, tempArray[i].starPos); // Get Distance Between two stars 
            
            if (tempArray[i].id != theStar.id && dis < (theStar.radius()+ tempArray[i].radius()) && tempArray[i].StarMass <= theStar.StarMass)
            {
                theStar.IncreaseMass(tempArray[i].StarMass);    
                            
                holdStars.Remove(tempArray[i]);
                restartCheck = true;
                break;
            }
        }

        //if the star has got bigger need to check all stars around it again
        if (restartCheck)
        {
            RemoveStarsInStar(theStar);
        }
       
    }


    /// <summary>
    /// for moving all stars towards a point
    /// </summary>
    /// <param name="moveTo"></param>
    /// <param name="massAtPoint"></param>
    public void moveStars(Vector3 moveTo,float massAtPoint)
    {
        for (int i = 0; i < holdStars.Count; i++)
        {
            float force = 10 * massAtPoint / holdStars[i].StarMass; // / Vector3.Distance(transform.position, s.transform.position * s.GetComponent<Star>().mass);

            holdStars[i].starPos = Vector3.MoveTowards(holdStars[i].starPos, moveTo, force);// * Time.deltaTime);
        }
      

        //We hold the list in an array temporerally so we can remove stars without error
        //as you cant remove items from a list as you cycle through it 
        HoldStar[] tempArray = holdStars.ToArray(); 
        foreach (HoldStar s in tempArray)
        {
            RemoveStarsInStar(s);
        }
    }

}



public enum starType
{
    BlackHole = 0,
    Neutron =    1,     
    SingeStar = 2,
    BinaryStar = 3,
    Ternarystar = 4,
}
