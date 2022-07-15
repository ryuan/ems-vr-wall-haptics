using UnityEngine;

public class MagnetWallSettings : PersistBehaviour
{
    /* Static Settings*/
    public static bool useStaticValues = false;
    public static int staticCurrent1 = 15;
    public static int staticWidth1 = 100;
    public static int staticWidth2 = 100;
    public static int staticCurrentBB1 = 15;
    public static int staticCurrentBB2 = 15;
    public static int staticMaxWidth1 = 100;
    public static int staticMaxWidth2 = 100;
    /**/

    public bool impacto;
    public bool outerBB;

    [Header("Impacto Style Settings")]
    //[Tooltip("in ms")]
    //public int duration = 100;

    [Tooltip("in ms")]
    public int timeout = 1000;

    [Tooltip("in mA")]
    public int pulseCurrent = 15;

    [Tooltip("in ms")]
    public int pulseWidth1 = 100;

    [Tooltip("in ms")]
    public int pulseWidth2 = 100;

    public bool enableDistortion1 = true;

    [Header("OuterBB Settings")]
    [Tooltip("in Hz")]
    public int frequencyBB = 75;

    [Tooltip("in mA")]
    public int pulseCurrentBB1 = 15;

    [Tooltip("in ms")]
    public int maxPulseWidth1 = 200;

    [Tooltip("in mA")]
    public int pulseCurrentBB2 = 15;

    [Tooltip("in ms")]
    public int maxPulseWidth2 = 200;

    public bool enableDistortion2 = true;

    [Header("Collider Objects")]
    public SolidWallColliderBehaviour outerCollider;

    public ShockColliderBehaviour impactoCollider;

    private void Start()
    {
        if (useStaticValues)
        {
            pulseCurrent = staticCurrent1;
            pulseWidth1 = staticWidth1;
            pulseWidth2 = staticWidth2;
            pulseCurrentBB1 = staticCurrentBB1;
            pulseCurrentBB2 = staticCurrentBB2;
            maxPulseWidth1 = staticMaxWidth1;
            maxPulseWidth2 = staticMaxWidth2;
            OnValidate();
        }
    }

    //
    private void OnValidate()
    {
        impactoCollider.gameObject.SetActive(impacto);
        //disable corresponding distortions
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.gameObject.tag == "Distortion1")
            {
                child.gameObject.SetActive(impacto && enableDistortion1);
            }
        }

        outerCollider.gameObject.SetActive(outerBB);
        //disable corresponding distortions
        children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            //print(child.gameObject.tag);
            if (child.gameObject.tag == "Distortion2")
            {
                child.gameObject.SetActive(
                    outerBB &&
                    enableDistortion2);
            }
        }

        impactoCollider.timeout = timeout;
        impactoCollider.pulseCurrent1 = pulseCurrent;
        impactoCollider.pulseWidth1 = pulseWidth1;
        impactoCollider.pulseCurrent2 = pulseCurrent;
        impactoCollider.pulseWidth2 = pulseWidth2;

        outerCollider.frequency = frequencyBB;
        outerCollider.pulseCurrent1 = pulseCurrentBB1;
        outerCollider.maxPulseWidth1 = maxPulseWidth1;
        outerCollider.pulseCurrent2 = pulseCurrentBB2;
        outerCollider.maxPulseWidth2 = maxPulseWidth2;
    }
}