using UnityEngine;
using System.Collections.Generic;

public class CreateGalaxy : MonoBehaviour
{
    public const int starMuti = 1;
    public int numStars = 100;
    public List<GameObject> stars = new List<GameObject>();
    public string starPrefabName = "StarPrefab";
   // private int spaceBetween = StarMuti*10;
    public GalaxyType galaxy;
    private Vector3 centerpos = Vector3.zero;
    bool testing = false;
    bool backhole = true;
    BlackHole black;

    // Use this for initialization
    void Start()
    {
        int max = 10000;
        Application.runInBackground = true;
        Debug.Log("need to add lod to planets");
        Vector3 starPos;
        int xPos = 50;
        int yPos = 0;
        int zPos = 25;
        int baseStarMass = 10;
                
       
        if (galaxy == GalaxyType.Test)
        {
            /*
           for (float a = 0; a < xPos; a++)
           {
               for (float b = 0; b < zPos; b++)
               {
                   float holdx = a / xPos *10;
                   float holdz = b / zPos * 10;

                   float sample = Mathf.PerlinNoise(holdx, holdz);
                   //Debug.Log(sample + " " + holdx + " " + holdz);
                   if(sample>0)
                   {
                       //CheckStarNear(xPos, zPos);
                       starPos = new Vector3(a*100, yPos, b * 100);
                       GameObject Star = Instantiate(Resources.Load(starPrefabName)) as GameObject;
                       Star.transform.SetParent(transform);

                       stars.Add(Star);
                       Star.transform.position = starPos;
                       Star.GetComponent<Star>().SetMass(Mathf.CeilToInt(sample* baseStarMass));

                   }

               }

           }
           */
            if (galaxy == GalaxyType.Circle)
            {
                int angle = Random.Range(0, 365);
                int radius = Random.Range(0, 10000);
                xPos = (int)(centerpos.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad));
                zPos = (int)(centerpos.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            }

            int radiusMax = 40;
            float radiusIncrease = 10;
            int angelMax = 360;

            if (galaxy == GalaxyType.CircleRand)
            {
                for (float radius = 0; radius < radiusMax * radiusIncrease; radius += radiusIncrease)
                {
                    // look at this and more rotaions 
                    for (float angle = 0; angle < angelMax; angle += (300 / radius))
                    {
                        // Debug.Log((150 / radius) + " " + radius);
                        //float holdx = a / xPos * 10;
                        // float holdz = b / zPos * 10;
                        float holdx = centerpos.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                        float holdz = centerpos.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

                        float sample = Random.Range(0.2f, 0.9f);//Mathf.PerlinNoise(holdx, holdz);                   
                                                                //Debug.Log(sample + " " + holdx + " " + holdz);
                        if (sample > 0)
                        {
                            //CheckStarNear(xPos, zPos);
                            starPos = new Vector3(holdx * 10, yPos, holdz * 10);
                            GameObject Star = Instantiate(Resources.Load(starPrefabName)) as GameObject;
                            Star.transform.SetParent(transform);

                            stars.Add(Star);
                            Star.transform.position = starPos;
                            Star.GetComponent<Star>().SetMass(Mathf.CeilToInt(sample * baseStarMass));

                        }

                    }
                }
            }




             radiusMax =50;
            radiusIncrease = 2;
             angelMax = 360;
            int spirals = 2;
            int sprialWidth = 50;
            float[] spiralAngel =new float[spirals];
            float angelhold = angelMax / spirals;
            for (int i = 0; i < spiralAngel.Length;i++)
            {

                spiralAngel[i] = i * angelhold;

            }



            float increase = 0;
            
            for (float c = 0; c < radiusMax ; c++)
            {          
                // look at this and more rotaions 
                for (float angle = 0; angle < angelMax; angle += increase)
                {

                    for (int i = 0; i < spiralAngel.Length; i++)
                    {
                       
                        if ((angle >= spiralAngel[i] && angle < spiralAngel[i] + sprialWidth || angle + 360 - sprialWidth <= spiralAngel[i] && angle + 360 > spiralAngel[i]))
                        {
                            float holdx = centerpos.x + (radiusIncrease * c)     * Mathf.Cos(angle * Mathf.Deg2Rad);
                            float holdz = centerpos.z + (radiusIncrease * c)   * Mathf.Sin(angle * Mathf.Deg2Rad);

                            float sample = Random.Range(0.4f, 0.8f);//Mathf.PerlinNoise(holdx, holdz);                   
                            GameObject Stara = null;                              //Debug.Log(sample + " " + holdx + " " + holdz);
                            if (sample > 0)
                            {
                                //CheckStarNear(xPos, zPos);
                                starPos = new Vector3(holdx * 10, yPos, holdz * 10);
                                Stara = Instantiate(Resources.Load(starPrefabName)) as GameObject;
                                Stara.transform.SetParent(transform);

                                stars.Add(Stara);
                                Stara.transform.position = starPos;
                                // if(angle> spiralAngel && angle< spiralAngel+ sprialWidth)
                                // {
                                //     Star.GetComponent<Star>().SetMass(Mathf.CeilToInt(sample * baseStarMass*2));

                                // }
                                // else
                                // {
                                Stara.GetComponent<Star>().SetMass(Mathf.CeilToInt(sample * baseStarMass));
                                Stara.GetComponent<Star>().angle = angle;
                                Stara.GetComponent<Star>().spiralAngel = spiralAngel[i];
                                //  }

                            }
                            i = spiralAngel.Length;

                        }
                    }




                   
                   

                    increase = 10;// (500 / radius);
                    
                    /*
                   
                    if (angle > spiralAngel && angle < spiralAngel + sprialWidth)
                    {
                        increase = (300 / radius);

                    }
                    else
                    {
                        increase = (600 / radius);

                    }
                    */


                }


                for (int i = 0; i < spiralAngel.Length; i++)
                {

                    spiralAngel[i] += radiusIncrease;

                    if (spiralAngel[i] >= angelMax)
                    {
                        spiralAngel[i] -= angelMax;
                    }
                    else if (spiralAngel[i] < 0)
                    {
                        spiralAngel[i] += angelMax;
                    }

                }


               


                //radiusIncrease = Random.Range(0.0f, 10.0f);
            }


        }
       //GameObject ablack = Instantiate(Resources.Load("BlackHole")) as GameObject;
        
       // ablack.transform.SetParent(transform);
        //black = ablack.GetComponent<BlackHole>();
    }
  

    GameObject CheckStarAlreadyThere(int xcord, int zcord, float spaceBetween)
    {

        foreach (GameObject s in stars)
        {
            // if theese are true than the 
            // if(s.transform.position.x -spaceBetween >xcord || s.transform.position.x + spaceBetween < xcord){}
           
            // if this is true then the planet is within the x of this
            if (s.transform.position.x - spaceBetween < xcord && s.transform.position.x + spaceBetween > xcord)
            {
                if (s.transform.position.z - spaceBetween < zcord && s.transform.position.z + spaceBetween > zcord)
                {
                   // Debug.Log(s.transform.position.x + " " + xcord + " " + s.transform.position.z + " " + zcord);
                    // if this is ever reached then there is an overlap
                    return s;
                }
            }

        }


        return null;

    }

    GameObject CheckStarAlreadyThere(float spaceBetween,Transform theStar)
    {

        foreach (GameObject s in stars)
        {
            if (s != theStar.gameObject)
            {
               // Debug.Log("1");
                // if theese are true than the 
                // if(s.transform.position.x -spaceBetween >xcord || s.transform.position.x + spaceBetween < xcord){}

                // if this is true then the planet is within the x of this
                if (s.transform.position.x - spaceBetween < theStar.position.x && s.transform.position.x + spaceBetween > theStar.position.x)
                {
                    if (s.transform.position.z - spaceBetween < theStar.position.z && s.transform.position.z + spaceBetween > theStar.position.z)
                    {
                        //Debug.Log(s.transform.position.x + " " + theStar.position.x + " " + s.transform.position.z + " " + theStar.position.z);
                        // if this is ever reached then there is an overlap
                        return s;
                    }
                }
                Debug.LogError(s.transform.position.x + " " + theStar.position.x + " " + s.transform.position.z + " " + theStar.position.z + " " + spaceBetween);

            }

        }


        return null;

    }



    void CheckStarNear(int xcord, int zcord)
    {
        int spaceNear = 100;
        foreach (GameObject s in stars)
        {
            // if this is true then the planet is within the x of this
            if (s.transform.position.x - spaceNear < xcord && s.transform.position.x + spaceNear > xcord)
            {
                if (s.transform.position.z - spaceNear < zcord && s.transform.position.z + spaceNear > zcord)
                {
                    s.name = "1";
                    //Debug.Log(s.transform.position.x + " " + xcord + " " + s.transform.position.z + " " + zcord);
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        float checkDist = -1;
        //if (Input.GetKey("h") && testing)
        if (testing)
        {
            bool checkmove = false;

            //List<GameObject> star2 = new List<GameObject>(stars);
            //List<GameObject> star2 = stars;
            //star2 = stars;
            foreach (GameObject s in stars)
            {
                Star sStar = s.GetComponent<Star>();
                Collider[] hold = Physics.OverlapSphere(s.transform.position, 100 + sStar.mass / 1);

                Vector3 holdPos = s.transform.position;
                //Debug.Log("1");
                // star2.Remove(s);
                foreach (Collider sa in hold)
                {
                    //Debug.Log("2");
                    Star saStar = sa.GetComponent<Star>();
                    if (saStar)
                    {
                        

                        // float force = 10000 * saStar.Mass / Vector3.Distance(sa.transform.position, s.transform.position * sStar.Mass);
                        //s.transform.position = Vector3.MoveTowards(s.transform.position, sa.transform.position, force * Time.deltaTime);
                        // Debug.Log(force);    
                        //Debug.Log(s.transform.position);
                        //Debug.Log(s.transform.position);
                        //Debug.Log(sa.transform.position);
                        float force = 1000 * sStar.Mass;

                        Vector3 pos = sa.transform.position;
                        pos *= saStar.Mass;
                        force /= Vector3.Distance(s.transform.position, pos);
                        sa.transform.position = Vector3.MoveTowards(sa.transform.position, s.transform.position, force * Time.deltaTime);
                        // Debug.Log(force);    

                    }

                }

                if (Vector3.Distance(holdPos, s.transform.position) > 0.0001f)
                {
                    checkmove = true;

                }
                else
                {
                    if (Vector3.Distance(holdPos, s.transform.position) > checkDist)
                    {

                        checkDist = Vector3.Distance(holdPos, s.transform.position);

                    }


                }

            }

            if (!checkmove)
            {
                checkmove = true;
                //Debug.Log("No more move " + checkDist);
            }


            if (!checkmove)
            {
                testing = false;
            }


            if (black)
            {

                foreach (GameObject s in stars)
                {

                    float force = 1000 * black.mass / Vector3.Distance(black.transform.position, s.transform.position * s.GetComponent<Star>().mass);
                     s.transform.position = Vector3.MoveTowards(s.transform.position, black.transform.position, force * Time.deltaTime);
                    // Debug.Log(force); 
                }

            }





        }

        if (Input.GetKey("h") && !testing)
        {
            Debug.Log("No more move");
        }

    }



    void OtherStarTypes(int max, int xPos,int zPos, int starMass)
    {
        int maxn = max;
        for (int i = 0; i < numStars; i++)
        {
            
            xPos = Random.Range(-maxn, max);
            int yPos = 0;
            //Check which type of glaxy you want to start from 
            if (galaxy == GalaxyType.square)
            {
            }
            else if (galaxy == GalaxyType.triangle)
            {
                max -= xPos;
                maxn -= xPos;

            }
            else if (galaxy == GalaxyType.Parallelogram)
            {
                max -= xPos;
                maxn += xPos;

            }
            else if (galaxy == GalaxyType.Arrow)
            {
                if (xPos > 0)
                {
                    max += xPos;
                    maxn -= xPos;
                }
                else
                {
                    max -= xPos;
                    maxn += xPos;
                }
            }
            zPos = Random.Range(-maxn, max);

            if (galaxy == GalaxyType.Circle)
            {
                int angle = Random.Range(0, 365);
                int radius = Random.Range(0, 10000);
                xPos = (int)(centerpos.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad));
                zPos = (int)(centerpos.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            }




            //if a star is already in teh area where this one would spawn that star gets this ones mass
            float hold = starMuti * (starMass);
            GameObject check = CheckStarAlreadyThere(xPos, zPos, hold);
            if (check)
            {
                Star cele = check.GetComponent<Star>();

                cele.IncreaseMass(starMass);

            }
            else
            {
                //CheckStarNear(xPos, zPos);
                Vector3 starPos = new Vector3(xPos, yPos, zPos);
                GameObject a = Instantiate(Resources.Load(starPrefabName)) as GameObject;
                a.transform.SetParent(transform);

                stars.Add(a);
                a.transform.position = starPos;
                a.GetComponent<Star>().SetMass(starMass);
            }

        }



    }
}



public enum GalaxyType
{
    square,
    triangle,
    Parallelogram,
    Arrow,
    Circle,
    CircleRand,
    Test




}