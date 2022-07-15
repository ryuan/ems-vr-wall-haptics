using UnityEngine;

/**
 * Script for the Punch Cube
 * Applies Impacto stimulation depending on the speed of the hand when entering the collider.
 * Needs to be childrened to the cube object.
 * The cube object needs to have a rigidbody attached to it.
 */

public class PunchColliderBehaviour : MonoBehaviour
{
    [Tooltip("How much to amplify the hand velocity")]
    public float forceFactor = 10;

    public float forceThreshhold = 0.5f;

    [Header("EMS  Settings")]
    public float velocityFactor = 10;

    [Header("Wrist")]
    public bool useWrist;

    [Tooltip("max Value in mA")]
    public int wristCurrent = 15;

    [Tooltip("in ms")]
    public int wristWidthL = 100;

    [Tooltip("in ms")]
    public int wristWidthR = 100;

    [Header("Biceps")]
    public bool useBiceps;

    [Tooltip("max Value in mA")]
    public int bicepsCurrent = 15;

    [Tooltip("in ms")]
    public int bicepsWidthL = 100;

    [Tooltip("in ms")]
    public int bicepsWidthR = 100;

    [Header("Shoulder")]
    public bool useShoulder;

    [Tooltip("max Value in mA")]
    public int shoulderCurrent = 15;

    [Tooltip("in ms")]
    public int shoulderWidthL = 100;

    [Tooltip("in ms")]
    public int shoulderWidthR = 100;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            transform.parent.GetComponent<Rigidbody>().AddForce(new Vector3(1, 0, 0) * forceFactor, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // check if the user's hand entered
        if (!
            (other.gameObject.CompareTag("Hand")
            || other.gameObject.CompareTag("HandR")
            || other.gameObject.CompareTag("HandL")
            )
        )
            return;

        // get the current velocity of the hand
        VelocityTracker script = other.gameObject.GetComponent<VelocityTracker>();
        if (script)
        {
            // apply impacto stimulation based on velocity
            Side side = other.gameObject.CompareTag("HandR") ?
                 Side.Right
                  : Side.Left;

            //script.velocity.magnitude;
            float w1 = getPulseWidth(script.velocity.magnitude, wristWidthL);
            if (useWrist)
            {
                int pulseWidth1 = (int)Mathf.Round(getPulseWidth(
                    script.velocity.magnitude
                    , side == Side.Left ? wristWidthL : wristWidthR
                    ));
                ArmStimulation.StimulateArmSinglePulse(Part.Wrist, side, pulseWidth1, wristCurrent);
            }
            if (useBiceps)
            {
                int pulseWidth2 = (int)Mathf.Round(getPulseWidth(
                    script.velocity.magnitude
                    , side == Side.Left ? bicepsWidthL : bicepsWidthR
                    ));
                ArmStimulation.StimulateArmSinglePulse(Part.Biceps, side, pulseWidth2, bicepsCurrent);
                print(pulseWidth2);
            }

            if (useShoulder)
            {
                int pulseWidth3 = (int)Mathf.Round(getPulseWidth(
                    script.velocity.magnitude
                    , side == Side.Left ? shoulderWidthL : shoulderWidthR
                    ));
                ArmStimulation.StimulateArmSinglePulse(Part.Shoulder, side, pulseWidth3, shoulderCurrent);
            }

            //apply force to rigidbody
            if ((script.velocity * forceFactor).magnitude > forceThreshhold)
                transform.parent.GetComponent<Rigidbody>().AddForce(script.velocity * forceFactor, ForceMode.Impulse);
        }
    }

    private float getPulseWidth(float magnitude, float maxValue)
    {
        return Mathf.Min(maxValue, Mathf.Max(0, Mathf.Pow(maxValue, magnitude * velocityFactor)));
    }
}