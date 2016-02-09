using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
    private float maxSpeed =1.0f;
    private float acceleration = 1.0f;
    public Vector3 velocity;
    private bool left ;
    private bool right;
    private bool forward;
    private bool back;
    private bool mouseHeld;

    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor;
    //public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
   
    // Use this for initialization
    void Start () {

        velocity = Vector3.zero;
        left = false;
        right = false;
        forward = false;
        back = false;
        mouseHeld = false;
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update () {

        CheckButton();       
        
        // if you hold the mous rotate the camera
        if (mouseHeld)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            var targetOrientation = Quaternion.Euler(targetDirection);

            //http://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/
            // Get raw mouse input for a cleaner reading on more sensitive mice.
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(smoothing.x, smoothing.y));

            // Interpolate mouse movement over time to apply smoothing delta.
            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

            // Find the absolute mouse movement value from point zero.
            _mouseAbsolute += _smoothMouse;

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (clampInDegrees.x < 360)
                _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

            var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
            transform.localRotation = xRotation;

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
                _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

            transform.localRotation *= targetOrientation;

            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }

        if (forward && velocity.x < maxSpeed)
        {
            velocity.x += acceleration * Time.deltaTime;

        }
        else if (!forward && velocity.x > 0)
        {
            velocity.x -= (acceleration + velocity.x / 2) * Time.deltaTime;
        }


        if (back && velocity.x > -maxSpeed)
        {
            velocity.x -= acceleration * Time.deltaTime;

        }
        else if (!back && velocity.x < 0)
        {
            velocity.x += (acceleration - velocity.x / 2) * Time.deltaTime;
        }

        if (left && velocity.z > -maxSpeed)
        {
            velocity.z -= acceleration * Time.deltaTime;
        }
        else if (!left && velocity.z < 0)
        {
            velocity.z += (acceleration - velocity.z / 2) * Time.deltaTime;

        }
        if (right && velocity.z < maxSpeed)
        {
            velocity.z += acceleration * Time.deltaTime;
        }
        else if (!right && velocity.z > 0)
        {
            velocity.z -= (acceleration +velocity.z/2 )* Time.deltaTime ;

        }


        if(velocity.z <0.01f && velocity.z > -0.01f)
        {

            velocity.z = 0;
        }
        if (velocity.x < 0.01f && velocity.x > -0.01f)
        {

            velocity.x = 0;
        }


        // move the camera in the diurection its facing 
        transform.position += transform.forward * velocity.x;
        transform.position += transform.right * velocity.z;



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
