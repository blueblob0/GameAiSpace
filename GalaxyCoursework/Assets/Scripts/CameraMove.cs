//script made by: up651590, except for the defined region code from  http://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/ 
using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
    private float maxSpeed; //=1.0f * CreateGalaxy.starMuti;
    private float acceleration = 0.5f * CreateGalaxy.starMuti;
    public Vector3 velocity;
    public bool left ;
    public bool right;
    public bool forward;
    public bool back;
    private bool mouseHeld;

    private float speedChange =10;
    private int starEntered = 0;
    private int planetEntered = 0;

   
   
    // Use this for initialization
    void Start () {
        maxSpeed = 1.0f * CreateGalaxy.starMuti;
        //Debug.Log(maxSpeed);
        velocity = Vector3.zero;
        left = false;
        right = false;
        forward = false;
        back = false;
        mouseHeld = false;
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;
    }

   
    Vector2 mouseAbsolute;
    Vector2 smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);

    //public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;

    // Update is called once per frame
    void Update () {
        //Debug.Log(maxSpeed);
        CheckButton();

        //region to show copied code
        #region camera code from http://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/ 
        // if you hold the mous rotate the camera
        if (mouseHeld)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            var targetOrientation = Quaternion.Euler(targetDirection);
            
            // Get raw mouse input for a cleaner reading on more sensitive mice.
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(smoothing.x, smoothing.y));

            // Interpolate mouse movement over time to apply smoothing delta.
            smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

            // Find the absolute mouse movement value from point zero.
            mouseAbsolute += smoothMouse;

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (clampInDegrees.x < 360)
                mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

            var xRotation = Quaternion.AngleAxis(-mouseAbsolute.y, targetOrientation * Vector3.right);
            transform.localRotation = xRotation;

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
                mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

            transform.localRotation *= targetOrientation;

            var yRotation = Quaternion.AngleAxis(mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        #endregion

        if (forward && velocity.x < maxSpeed)
        {
            velocity.x += acceleration * Time.deltaTime;
            //Debug.Log("14");
        }
        else if (!forward && velocity.x > 0)
        {
            velocity.x -= (acceleration + velocity.x ) * Time.deltaTime;
            //Debug.Log("13");
        }


        if (back && velocity.x > -maxSpeed)
        {
            velocity.x -= acceleration * Time.deltaTime;
            //Debug.Log("12");

        }
        else if (!back && velocity.x < 0)
        {
            //Debug.Log("11");
            velocity.x += (acceleration - velocity.x ) * Time.deltaTime;
        }

        if (left && velocity.z > -maxSpeed)
        {
            velocity.z -= acceleration * Time.deltaTime;
        }
        else if (!left && velocity.z < 0)
        {
            velocity.z += (acceleration - velocity.z ) * Time.deltaTime;

        }
        if (right && velocity.z < maxSpeed)
        {
            velocity.z += acceleration * Time.deltaTime;
        }
        else if (!right && velocity.z > 0)
        {
            velocity.z -= (acceleration +velocity.z )* Time.deltaTime ;

        }


        if(velocity.z < acceleration / 100 && velocity.z > -acceleration / 100)
        {

            velocity.z = 0;
        }

        if (velocity.x < (acceleration / 100) && velocity.x > (-acceleration / 100))
        {

            velocity.x = 0;
        }


        // move the camera in the diurection its facing 
        transform.position += transform.forward * velocity.x;
        transform.position += transform.right * velocity.z;



    }
   


    public void DecreaseSpeedPlanet()
    {
        planetEntered++;
        if(planetEntered == 1) // if this is the first planet entered
        {
            maxSpeed /= speedChange;
            acceleration /= speedChange;

            //gameObject.GetComponent<SphereCollider>().radius /= speedChange;
            if (velocity.x > maxSpeed)
            {
                velocity.x = maxSpeed;
            }
            if (velocity.z > maxSpeed)
            {
                velocity.z = maxSpeed;
            }

        }

        
    }

    public void DecreaseSpeedStar()
    {
        starEntered++;
        if (starEntered == 1) // if this is the first star entered
        {
            maxSpeed /= speedChange;
            acceleration /= speedChange;

            //gameObject.GetComponent<SphereCollider>().radius /= speedChange;
            if (velocity.x > maxSpeed)
            {
                velocity.x = maxSpeed;
            }
            if (velocity.z > maxSpeed)
            {
                velocity.z = maxSpeed;
            }

        }


       
    }

    public void IncreaseStarSpeed()
    {
        starEntered--;
        if (starEntered == 0) // if this is the last star exited
        {
            maxSpeed *= speedChange;
            acceleration *= speedChange;
        }


    }

    public void IncreasePlanetSpeed()
    {

        planetEntered--;
        if (planetEntered == 0) // if this is the last planet entered
        {
            maxSpeed *= speedChange;
            acceleration *= speedChange;
        }
    }

    private void CheckButton()
    {
        
        left = Input.GetKey(KeyCode.A);
        right = Input.GetKey(KeyCode.D);   
        forward = Input.GetKey(KeyCode.W);
        back = Input.GetKey(KeyCode.S);
        mouseHeld = Input.GetMouseButton(1);
    }
    /*
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("HIT");
    }
    */
}
