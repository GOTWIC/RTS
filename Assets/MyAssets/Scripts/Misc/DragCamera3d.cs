using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragCamera3d : MonoBehaviour
{
    /*
     *TODO: 
     *  DONE: replace dolly with bezier dolly system
     *  DONE: add dolly track smoothing
     *  DONE: add dolly track straightening
     *  DONE: Dolly track + gizmo colours
     *  DONE: add non tracked constant speed dolly system(continuous movement based on time)
     *  WONTDO: [REPLACED BY FEATURE BELOW] add button to split dolly track evenly (between start and end) for time based dolly movement
     *  DONE: button to adjust times on all waypoints so camera moves at a constant speed
     *  DONE: add per waypoint time (seconds on this segment)
     *  DONE: add scaler for time to next waypoint in scene viewe gui
     *  DONE: improve GUI elements (full custom editor inspector)
     *  DONE:    add waypoint gui  scene view button
     *  DONE: better designed example scenes
     *  DONE: option to lock camera to track even if object escapes area
     *  add multiple dolly tracks to allow creating loops etc
     *  add track change triggers
     *  add bounds ids for multiple bounds
     *  add bounds triggers(e.g. small bounds until x event(obtain key etc) then larger bounds
     *  add configurable keymap to allow developers/usres to map keys to actions
     *  DONE: add in scene dolly track controls
     *  possibly add event system for lerping camera to position
     *  possibly make dolly track event system to allow camera to track dolly after an event then return to user control(for cutscenes/tutorial etc)
     *  
     *  Requests:
     *  ADDED: Zoom/Translate to double click position
     *  ADDED:  Translate to double click
     *  ADDED: Zoom to double click
     *  TODO: Scroll Snapping
     *  
     *  Bugfixes:
     *  The name does not exists in current content during build : fix supplied by  @chimerian
     *  zoom to mouse would translate camera position even when fully zoomed in our out. FIXED
     *  
     *  BUGS TO FIX:
     *  Double clicking area restricted by area clamp locks camera in lerp to double click target
    */

    public Camera cam;

    [Header("Camera Movement")]

    [Tooltip("Allow the Camera to be dragged.")]
    public bool dragEnabled = true;
    [Tooltip("Mouse button responsible for drag.")]
    public MouseButton mouseButton;
    [Range(-5, 5)]
    [Tooltip("Speed the camera moves when dragged.")]
    public float dragSpeed = -0.06f;

    [Header("Edge Scrolling")]
    [Tooltip("Pixel Border to trigger edge scrolling")]
    public int edgeBoundary = 20;
    [Range(0, 10)]
    [Tooltip("Speed the camera moves Mouse enters screen edge.")]
    public float edgeSpeed = 1f;

    [Header("Touch(PRO) & Keyboard Input")]
    [Tooltip("Enable or disable Keyboard input")]
    public bool keyboardInput = false;
    [Tooltip("Invert keyboard direction")]
    public bool inverseKeyboard = false;
    [Tooltip("Enable or disable touch input")]
    public bool touchEnabled = false;
    [Tooltip("Drag Speed for touch controls")]
    [Range(-5, 5)]
    public float touchDragSpeed = -0.03f;

    [Header("Zoom")]
    [Tooltip("Enable or disable zooming")]
    public bool zoomEnabled = true;
    [Tooltip("Scale drag movement with zoom level")]
    public bool linkedZoomDrag = true;
    [Tooltip("Maximum Zoom Level")]
    public float maxZoom = 10;
    [Tooltip("Minimum Zoom Level")]
    [Range(0.01f, 10)]
    public float minZoom = 0.5f;
    [Tooltip("The Speed the zoom changes")]
    [Range(0.1f, 10f)]
    public float zoomStepSize = 0.5f;
    [Tooltip("Enable Zooming to mouse pointer")]
    public bool zoomToMouse = false;




    [Header("Follow Object")]
    public GameObject followTarget;
    [Range(0.01f, 1f)]
    public float lerpSpeed = 0.5f;
    public Vector3 offset = new Vector3(0, 0, -10);


    [Header("Camera Bounds")]
    public bool clampCamera = true;
    public CameraBounds bounds;

    public const double PI = 3.1415926535897931;

    float radians = 0;


    //hidden
    [HideInInspector]
    public enum MouseButton
    {
        Left = 0,
        Middle = 2,
        Right = 1
    }

    // private vars
    Vector3 bl;
    Vector3 tr;
    private Vector2 touchOrigin = -Vector2.one;


    int frameid = 0;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }


        radians = (float)(cam.transform.localRotation.eulerAngles.x * PI / 180);
    }

    void LateUpdate()
    {
        frameid++;
        if (dragEnabled)
        {
            panControl();
        }

        if (edgeBoundary > 0)
        {
            edgeScroll();
        }


        if (zoomEnabled)
        {
            zoomControl();
        }

        else
        {
            if (followTarget != null)
            {
                transform.position = Vector3.Lerp(transform.position, followTarget.transform.position + offset, lerpSpeed);
            }
        }

        if (clampCamera)
        {
            cameraClamp();
        }

        if (touchEnabled)
        {
            doTouchControls();
        }
    }

    private void edgeScroll()
    {
        float x = 0;
        float y = 0;
        

        if (Mouse.current.position.x.ReadValue() >= Screen.width - edgeBoundary)
        {
            // Move the camera
            x = Time.deltaTime * edgeSpeed;
        }
        if (Mouse.current.position.x.ReadValue() <= 0 + edgeBoundary)
        {
            // Move the camera
            x = Time.deltaTime * -edgeSpeed;
        }
        if (Mouse.current.position.y.ReadValue() >= Screen.height - edgeBoundary)
        {
            // Move the camera
            y = Time.deltaTime * edgeSpeed
;
        }
        if (Mouse.current.position.y.ReadValue() <= 0 + edgeBoundary)
        {
            // Move the camera
            y = Time.deltaTime * -edgeSpeed
;
        }
        transform.Translate(x, y * Mathf.Sin(radians), y * Mathf.Cos(radians));
    }

    public void addCameraBounds()
    {
        if (bounds == null)
        {
            GameObject go = new GameObject("CameraBounds");
            CameraBounds cb = go.AddComponent<CameraBounds>();
            cb.guiColour = new Color(0, 0, 1f, 0.1f);
            cb.pointa = new Vector3(20, 0, 20);
            this.bounds = cb;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }

    public void doTouchControls()
    {
        // PRO Only
    }

    
    //click and drag
    public void panControl()
    {
        /*
        // if keyboard input is allowed
        if (keyboardInput)
        {
            float x = -Input.GetAxis("Horizontal") * dragSpeed;
            float y = -Input.GetAxis("Vertical") * dragSpeed;

            if (linkedZoomDrag)
            {
                x *= Camera.main.orthographicSize;
                y *= Camera.main.orthographicSize;
            }

            if (inverseKeyboard)
            {
                x = -x;
                y = -y;
            }

            transform.Translate(x, y * Mathf.Sin(radians), y * Mathf.Cos(radians));
        }

        */



        // if mouse is down
        if (Mouse.current.middleButton.isPressed)
        {
            float x = Mouse.current.delta.x.ReadValue() * dragSpeed;
            float y = Mouse.current.delta.y.ReadValue() * dragSpeed;

            if (linkedZoomDrag)
            {
                x *= Camera.main.orthographicSize;
                y *= Camera.main.orthographicSize;
            }

            transform.Translate(x, y * Mathf.Sin(radians), y * Mathf.Cos(radians));
        }


    }
    
    private void clampZoom()
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        Mathf.Max(cam.orthographicSize, 0.1f);


    }

    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        // Calculate how much we will have to move towards the zoomTowards position
        float multiplier = (1.0f / Camera.main.orthographicSize * amount);
        // Move camera
        transform.position += (zoomTowards - transform.position) * multiplier;
        // Zoom camera
        Camera.main.orthographicSize -= amount;
        // Limit zoom
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
    }

    // managae zooming
    public void zoomControl()
    {
        Vector2 vec = Mouse.current.scroll.ReadValue();
        float scroll = vec.y;


        if (zoomToMouse)
        {
            Vector3 mousePosition = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0);
            if (scroll > 0 && minZoom < Camera.main.orthographicSize) // forward
            {
                ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(mousePosition), zoomStepSize);
            }
            if (scroll < 0 && maxZoom > Camera.main.orthographicSize) // back            
            {
                ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(mousePosition), -zoomStepSize);
            }

        }
        else
        {

            if (scroll > 0 && minZoom < Camera.main.orthographicSize) // forward
            {
                Camera.main.orthographicSize = Camera.main.orthographicSize - zoomStepSize;
            }

            if (scroll < 0 && maxZoom > Camera.main.orthographicSize) // back            
            {
                Camera.main.orthographicSize = Camera.main.orthographicSize + zoomStepSize;
            }
        }
        clampZoom();
    }


    private bool lfxmax = false;
    private bool lfxmin = false;
    private bool lfymax = false;
    private bool lfymin = false;

    // Clamp Camera to bounds
    private void cameraClamp()
    {
        tr = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, -transform.position.z));
        bl = cam.ScreenToWorldPoint(new Vector3(0, 0, -transform.position.z));

        if (bounds == null)
        {
            //Debug.Log("Clamp Camera Enabled but no Bounds has been set.");
            return;
        }

        float boundsMaxX = bounds.pointa.x;
        float boundsMinX = bounds.transform.position.x;
        float boundsMaxY = bounds.pointa.y;
        float boundsMinY = bounds.transform.position.y;

        if (tr.x > boundsMaxX && bl.x < boundsMinX)
        {
            Debug.Log("User tried to zoom out past x axis bounds - locked to bounds");
            Camera.main.orthographicSize = Camera.main.orthographicSize - zoomStepSize; // zoomControl in to compensate
            clampZoom();
        }

        if (tr.y > boundsMaxY && bl.y < boundsMinY)
        {
            Debug.Log("User tried to zoom out past y axis bounds - locked to bounds");
            Camera.main.orthographicSize = Camera.main.orthographicSize - zoomStepSize; // zoomControl in to compensate
            clampZoom();
        }

        bool tfxmax = false;
        bool tfxmin = false;
        bool tfymax = false;
        bool tfymin = false;

        if (tr.x > boundsMaxX)
        {
            if (lfxmin)
            {
                Camera.main.orthographicSize = Camera.main.orthographicSize - zoomStepSize; // zoomControl in to compensate
                clampZoom();
            }
            else
            {
                transform.position = new Vector3(transform.position.x - (tr.x - boundsMaxX), transform.position.y, transform.position.z);
                tfxmax = true;
            }
        }
        if (tr.y > boundsMaxY)
        {
            if (lfymin)
            {
                Camera.main.orthographicSize = Camera.main.orthographicSize - zoomStepSize; // zoomControl in to compensate
                clampZoom();
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - (tr.y - boundsMaxY), transform.position.z);
                tfymax = true;
            }
        }
        if (bl.x < boundsMinX)
        {
            if (lfxmax)
            {
                Camera.main.orthographicSize = Camera.main.orthographicSize - zoomStepSize; // zoomControl in to compensate
                clampZoom();
            }
            else
            {
                transform.position = new Vector3(transform.position.x + (boundsMinX - bl.x), transform.position.y, transform.position.z);
                tfxmin = true;
            }
        }
        if (bl.y < boundsMinY)
        {
            if (lfymax)
            {
                Camera.main.orthographicSize = Camera.main.orthographicSize - zoomStepSize; // zoomControl in to compensate
                clampZoom();
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + (boundsMinY - bl.y), transform.position.z);
                tfymin = true;
            }
        }

        lfxmax = tfxmax;
        lfxmin = tfxmin;
        lfymax = tfymax;
        lfymin = tfymin;
    }
}
