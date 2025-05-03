using System.Xml.Serialization;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    public Transform trans;

    [Header("X Bounds")]
    public float minimumX = -70;
    public float maximumX = 70;

    [Header("Y Bounds")]
    public float minimumY = 18;
    public float maximumY = 80;

    [Header("Z Bounds")]
    public float minimumZ = -130;
    public float maximumZ = 70;

    [Tooltip("Distance traveled per second with the arrow keys")]
    public float arrowKeySpeed = 80;

    [Tooltip("Multiplier for mouse drag movement. A higher value will result in the camera moving a greater distance when the mouse is moved.")]
    public float mouseDragSensitivity = 2.8f;

    [Tooltip("Amount of smoothing applied to camera movement. Should be a value between 0 and .99f")]
    [Range(0, .99f)]
    public float movementSmoothing = .9f;

    //Current position the camera is moving towards:
    private Vector3 targetPosition;

    [Header("Scrolling")]
    [Tooltip("Amount of Y distance the camera moves per mouse scroll increment.")]
    public float scrollSensitivity = 1.6f;



    void ArrowKeyMovement()
    {
        //If up arrow is held,
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //...add to target Z position:
            targetPosition.z += arrowKeySpeed * Time.deltaTime;
        }
        //Otherwise, if down arrow is held,
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //...subtract to target Z position:
            targetPosition.z -= arrowKeySpeed * Time.deltaTime;
        }
        //If right arrow is held,
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //...add to target X position:
            targetPosition.x += arrowKeySpeed * Time.deltaTime;
        }
        //Otherwise, if left arrow is held,
        else if (!Input.GetKey(KeyCode.LeftArrow))
        {
            //...subtract from target X position:
            targetPosition.x -= arrowKeySpeed * Time.deltaTime;
        }
    }

    void MouseDragMovement()
    {
        //IF the right mouse button is held,
        if (Input.GetMouseButtonDown(1))
        {
            //Get the movement amount this frame:
            Vector3 movement = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * mouseDragSensitivity;
            //If there is any movement,
            if (movement != Vector3.zero)
            {
                //...apply it to the target position:
                targetPosition += movement;
            }
        }
    }

    void Zooming()
    {
        //Get the scroll delta Y value and flip it:
        float scrollData = -Input.mouseScrollDelta.y;

        //If there was any delta,
        if(scrollData != 0)
        {
            //...apply it to the Y position:
            targetPosition.y += scrollData * scrollSensitivity;
        }
    }

    void MoveTowardsTarget()
    {
        //Clamp the target position to the bounds variables:
        targetPosition.x = Mathf.Clamp(targetPosition.x, minimumX, maximumX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minimumY, maximumY);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minimumZ, maximumZ);

        //If we aren't already at the target position, move towards it:
        if(trans.position != targetPosition)
        {
            trans.position = Vector3.Slerp(trans.position, targetPosition, 1-movementSmoothing);
        }
    }


    //Events
    void Start()
    {
        targetPosition = trans.position;
    }

    void Update()
    {
        ArrowKeyMovement();
        MouseDragMovement();
        Zooming();
        MoveTowardsTarget();
    }
}
