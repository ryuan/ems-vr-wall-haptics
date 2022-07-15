using System.Collections;
using UnityEngine;
using MuscleDeck;

public class DoorControl : MonoBehaviour
{
    public float slideUpAmount = 2.6f;
    public float slideSpeed = 0.03f;

    public ButtonController[] buttons;

    public KeyCode toggleKey = KeyCode.Alpha0;

    private string currentState = "slidedDown";
    private float initialDoorHeight;
    private float targetDoorHeight;

    //private int buttonsPressed = 0;
    public RoomTeleporter teleporter;

    // Use this for initialization
    private void Start()
    {
        foreach (ButtonController button in buttons)
        {
            button.OnPressStateChange += reEvaluateButtonStates;
        }

        initialDoorHeight = transform.localPosition.y;
        targetDoorHeight = transform.localPosition.y + slideUpAmount;
    }

    protected bool allButtonsPressed = false;

    // Called when one button is pressed / unpressed
    private void reEvaluateButtonStates(bool newPressState)
    {
        //Debug.Log("Re-evaluating door state");

        // Check if all buttons are pressed
        allButtonsPressed = true;

        foreach (ButtonController button in buttons)
        {
            allButtonsPressed &= button.isPressed;
        }

        if (allButtonsPressed)
        {
            currentState = "slidingUp";
            GetComponent<AudioSource>().Play();
            //Debug.Log("New door state: slidingUp");
            if (teleporter != null)
            {
                teleporter.canTeleport = true;
            }
        }
        else if (currentState == "slidedUp" || currentState == "slidingUp")
        {
            currentState = "slidingDown";
            GetComponent<AudioSource>().Play();
            //Debug.Log("New door state: slidingDown");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(toggleKey))
        {
            GetComponent<AudioSource>().Play();
            if (currentState == "slidedUp")
            {
                currentState = "slidingDown";
            }
            else if (currentState == "slidedDown")
            {
                currentState = "slidingUp";
                teleporter.canTeleport = true;
                Debug.Log("New door state: slidingDown");
            }
            Debug.Log("New door state: " + currentState);
        }

        if (currentState == "slidedUp" || currentState == "slidedDown")
        {
            return;
        }

        float currentDoorHeight = transform.localPosition.y;
        float newDoorHeight = currentDoorHeight;

        switch (currentState)
        {
            case "slidingUp":
                if (currentDoorHeight >= targetDoorHeight)
                {
                    currentState = "slidedUp";
                    Debug.Log("New door state: slidedUp");
                    break;
                }
                newDoorHeight = currentDoorHeight + slideSpeed;
                break;

            case "slidingDown":
                if (currentDoorHeight <= initialDoorHeight)
                {
                    currentState = "slidedDown";
                    Debug.Log("New door state: slidedDown");
                    break;
                }
                newDoorHeight = currentDoorHeight - slideSpeed;
                break;
        }

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            newDoorHeight,
            transform.localPosition.z
        );
    }
}