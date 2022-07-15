using UnityEngine;

public class SolidWallSettings : MonoBehaviour
{
    /* Static Settings*/
    public static bool useStaticValues = false;
    public static int staticCurrent1 = 15;
    public static int staticCurrent2 = 15;
    public static int staticWidth1 = 100;
    public static int staticWidth2 = 100;
    /**/

    public int pulseCurrent1 = 15;
    public int pulseCurrent2 = 15;
    public int maxPulseWidth1 = 100;
    public int maxPulseWidth2 = 100;

    // Use this for initialization
    private void Start()
    {
        //Global settings
        if (useStaticValues)
        {
            pulseCurrent1 = staticCurrent1;
            pulseCurrent2 = staticCurrent2;
            maxPulseWidth1 = staticWidth1;
            maxPulseWidth2 = staticWidth2;
            OnValidate();
        }
    }

    private void OnValidate()
    {
        SolidWallColliderBehaviour[] children
            = GetComponentsInChildren<SolidWallColliderBehaviour>();

        foreach (SolidWallColliderBehaviour child in children)
        {
            child.pulseCurrent1 = pulseCurrent1;
            child.pulseCurrent2 = pulseCurrent2;
            child.maxPulseWidth1 = maxPulseWidth1;
            child.maxPulseWidth2 = maxPulseWidth2;
        }
    }
}