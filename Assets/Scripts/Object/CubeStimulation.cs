using System.Collections;
using UnityEngine;

public class CubeStimulation : MonoBehaviour
{
    [Header("EMS  Settings")]
    [Tooltip("How to scale distance to pulseWidth")]
    public scalingMode mode = scalingMode.linear;

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

    public enum scalingMode
    {
        linear = 1,
        //quadratic = 2
    };

    [Header("Scaling Stuff")]
    public int offset = 50;

    [Header("Linear Scaling Settings")]
    public float factor = 1;

    private float timer;
    private float cooldown = .2f;
    private float maxDistance = 1.0f;

    private new GameObject camera;

    // Use this for initialization
    private void Start()
    {
        camera = GameObject.Find("Camera");
    }

    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!
            (other.gameObject.CompareTag("Hand")
            || other.gameObject.CompareTag("HandR")
            || other.gameObject.CompareTag("HandL")
            )
        )
            return;

        //save maxDistance
        maxDistance = getDistanceToPoint(other.transform.position);
        maxDistance = Mathf.Abs(maxDistance);

        //Vector3 diff = transform.InverseTransformPoint(other.transform.position);

        //float x = Mathf.Abs(diff.x);
        //float y = Mathf.Abs(diff.y);
        //float z = Mathf.Abs(diff.z);
        useWrist = false;
        useBiceps = false;
        useShoulder = true;

        // hand object blue axis points from the back of the hand

        float upAngle = Vector3.Angle(camera.transform.up, other.transform.forward) - 90;
        float sideAngle = Vector3.Angle(camera.transform.right, other.transform.forward) - 90;
        float forwardAngle = Vector3.Angle(camera.transform.forward, other.transform.forward) - 90;
        useWrist = false;
        useBiceps = false;
        useShoulder = false;
        //print(forwardAngle);
        //print(upAngle);
        //print(sideAngle);
        float absMax = Mathf.Max(
                Mathf.Abs(forwardAngle),
                Mathf.Abs(upAngle),
                Mathf.Abs(sideAngle)
        );

        if (absMax == Mathf.Abs(forwardAngle))
        {
            //hand
            if (forwardAngle < 0)
            {
                // palm looks to camera
                useWrist = true;
            }
            else
            {
                // palm looks away to camera
                useWrist = true;
                useBiceps = true;
            }
        }
        else if (absMax == Mathf.Abs(upAngle))
        {
            if (upAngle < 0)
            {
                //palm looks down
                useWrist = true;
                useBiceps = true;
            }
            else
            {
                //palm looks up
                useWrist = true;
                //useTriceps = true;
            }
        }
        else if (absMax == Mathf.Abs(sideAngle))
        {
            if (sideAngle < 0)
            {
                //palm looks to left
                useShoulder = true;
            }
            else
            {
                //palm to right
                useShoulder = true;
            }
        }

        //if (useWrist)
        //{
        //    print("useWrist");
        //}
        //if (useBiceps)
        //{
        //    print("useBiceps");
        //}
        //if (useShoulder)
        //{
        //    print("useShoulder");
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (!
            (other.gameObject.CompareTag("Hand")
            || other.gameObject.CompareTag("HandR")
            || other.gameObject.CompareTag("HandL")
            )
            || timer > 0
        )
            return;

        // TODO check which hand
        Side side = other.gameObject.CompareTag("HandR") ?
            Side.Right
            : Side.Left;

        //check distance
        float distance = getDistanceToPoint(other.transform.position);

        //pulse width modulation
        if (useWrist)
        {
            int pulseWidth1 = (int)Mathf.Round(getPulseWidth(
                distance
                , maxDistance
                , side == Side.Left ? wristWidthL : wristWidthR
                ));
            ArmStimulation.StimulateArm(Part.Wrist, side, pulseWidth1, wristCurrent);
        }
        if (useBiceps)
        {
            int pulseWidth2 = (int)Mathf.Round(getPulseWidth(
                distance
                , maxDistance
                , side == Side.Left ? bicepsWidthL : bicepsWidthR
                ));
            ArmStimulation.StimulateArm(Part.Biceps, side, pulseWidth2, bicepsCurrent);
        }

        if (useShoulder)
        {
            int pulseWidth3 = (int)Mathf.Round(getPulseWidth(
                distance
                , maxDistance
                , side == Side.Left ? shoulderWidthL : shoulderWidthR
                ));
            ArmStimulation.StimulateArm(Part.Shoulder, side, pulseWidth3, shoulderCurrent);
        }
        timer = cooldown;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!
            (other.gameObject.CompareTag("Hand")
            || other.gameObject.CompareTag("HandR")
            || other.gameObject.CompareTag("HandL")
            )
        )
            return;

        ChannelList.Stop();
    }

    private float getPulseWidth(float distance, float maxDistance, float maxValue)
    {
        float factor;

        switch (mode)
        {
            case scalingMode.linear:
                factor = getLinearScaling(distance, maxDistance);
                break;

            //case scalingMode.quadratic:
            //    factor = getQuadraticScaling(distance, maxDistance);
            //    break;

            default:
                return 0;
        }

        return Mathf.Max(0, factor * maxValue + offset);
    }

    private float getDistanceToPoint(Vector3 point)
    {
        return Vector3.Distance(transform.position, point);
    }

    /**
     * @return a value between 0 and 1
     */

    private float getLinearScaling(float distance, float maxDistance)
    {
        return Mathf.Min(1, (1 - (distance / maxDistance)) * factor);
    }

    private float getQuadraticScaling(float distance, float maxDistance)
    {
        return 0.0f;
        //return Mathf.Min(1, 1 - Mathf.Pow((distance / maxDistance), power));
    }
}