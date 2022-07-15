using UnityEngine;

/**
 * Script for Slider
 */

public class Slider : PersistBehaviour
{
    [Header("Physic Settings")]
    public float speed;

    private int direction = 0;

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

    /*
     * Private Attributes
     */

    private float maxDistance = 1.0f;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //switch (direction)
        //{
        //    case -1:
        //        transform.Translate(-Vector3.forward * Time.deltaTime * speed);
        //        break;

        //    case 0:
        //        break;

        //    case 1:
        //        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //        break;

        //    default:
        //        break;
        //}
        transform.Translate(direction * Vector3.forward * Time.deltaTime * speed);
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

        Vector3 diff = transform.InverseTransformPoint(other.transform.position);

        float x = Mathf.Abs(diff.x);
        float y = Mathf.Abs(diff.y);
        float z = Mathf.Abs(diff.z);

        //print(x + "|" + y + "|" + z);

        useWrist = false;
        useBiceps = false;
        useShoulder = false;

        // approximate direction
        float max = Mathf.Max(new float[] { x, y, z });
        if (max == x)
        {
            if (diff.x > 0)
            {//from the front
                //useWrist = true;
                //useBiceps = true;

                useWrist = true;
                useBiceps = true;
            }
            else
            {//from the back
             //should not happen

                useWrist = false;
                useShoulder = false;
            }
        }
        else if (max == y)
        {
            if (diff.y > 0)
            {//from above
                useWrist = true;
                useBiceps = true;
            }
            else
            {//from below
                useWrist = true;
            }
        }
        else
        {
            if (diff.z > 0)
            {//from the right
                useWrist = true;
                useShoulder = true;
                direction = -1;
                GetComponent<AudioSource>().Play();
            }
            else
            {//from the left
                useWrist = true;
                useShoulder = true;
                direction = 1;
                GetComponent<AudioSource>().Play();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!
            (other.gameObject.CompareTag("Hand")
            || other.gameObject.CompareTag("HandR")
            || other.gameObject.CompareTag("HandL")
            )
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

        GetComponent<AudioSource>().Stop();
        direction = 0;

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