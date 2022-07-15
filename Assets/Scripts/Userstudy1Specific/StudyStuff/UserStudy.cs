using UnityEngine;

public class UserStudy : MonoBehaviour
{
    public int repetition = 1;
    public int question = 1;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            repetition = 1;
            question = 1;
            Logger.LogLine("Starting next Object");
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GlobalWallSettings ems = gameObject.GetComponent<GlobalWallSettings>();
            Logger.LogLine("EMS Settings: "
                + "SolidCurrent1: " + ems.solid_staticCurrent1
                + ",SolidCurrent2: " + ems.solid_staticCurrent2
                + ",SolidWidth1: " + ems.solid_staticWidth1
                + ",SolidWidth2: " + ems.solid_staticWidth2
                + ",ShockCurrent1: " + ems.shock_staticCurrent1
                + ",ShockWidth1: " + ems.shock_staticWidth1
                + ",ShockWidth2: " + ems.shock_staticWidth2
                );
        }
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            repetition++;
            question = 1;
            Logger.LogLine("Starting next repetition.");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Logger.LogLine("Repetition: " + repetition + ", Question: " + question + ", Rating: 1");
            question++;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Logger.LogLine("Repetition: " + repetition + ", Question: " + question + ", Rating: 2");
            question++;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Logger.LogLine("Repetition: " + repetition + ", Question: " + question + ", Rating: 3");
            question++;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Logger.LogLine("Repetition: " + repetition + ", Question: " + question + ", Rating: 4");
            question++;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            Logger.LogLine("Repetition: " + repetition + ", Question: " + question + ", Rating: 5");
            question++;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Logger.LogLine("Repetition: " + repetition + ", Question: " + question + ", Rating: 6");
            question++;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Logger.LogLine("Repetition: " + repetition + ", Question: " + question + ", Rating: 7");
            question++;
        }
    }
}