using System;
using UnityEngine;

public class SolidWallColliderBehaviour : PersistBehaviour
{
    [Header("Haptics")]
    public bool useHaptics = true;

    [Range(101, 250)]
    public int vibroFrequency = 200;

    [Header("EMS  Settings")]
    //public int pulseCount = 5;

    [Tooltip("in Hz")]
    public int frequency = 75;

    [Tooltip("How to scale distance to pulseWidth")]
    public scalingMode mode = scalingMode.linear;

    public int offset = 50;

    [Header("Linear Scaling Settings")]
    public float factor = 1;

    [Header("Quadratic Scaling Settings")]
    public float power = 4;

    [Header("Channel1")]
    public Channels channelNumber1 = Channels.Channel1;

    [Tooltip("in mA")]
    public int pulseCurrent1 = 15;

    [Tooltip("in ms")]
    public int maxPulseWidth1 = 100;

    [Header("Channel2")]
    public Channels channelNumber2 = Channels.Channel2;

    [Tooltip("in mA")]
    public int pulseCurrent2 = 15;

    [Tooltip("in ms")]
    public int maxPulseWidth2 = 100;

    private float maxDistance = 1f;

    public enum scalingMode
    {
        linear = 1,
        quadratic = 2
    };

    private bool stimulate = true;

    private void Stoptouch()
    {
        if (useHaptics)
        {
            //SerialInterface.SendMessage("-51");
            SerialInterface.SendMessage("-1");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Hand"))
            return;

        timer = DateTime.Now.AddSeconds(2);
        //save maxDistance
        maxDistance = getDistanceToPoint(other.transform.position);
        maxDistance = Mathf.Abs(maxDistance);

        //reset penetration
        maxPenetration = 0.0f;

        //sanity check
        stimulate = (maxDistance > 0.1f);

        if (stimulate && useHaptics)
        {
            //SerialInterface.SendMessage((-vibroFrequency).ToString());
            SerialInterface.SendMessage("-11");
        }

        //init EMS
        ChannelMask channels = new ChannelMask();
        channels.setChannel(channelNumber1);
        channels.setChannel(channelNumber2);

        ChannelList.InitCM(0
                , channels
                , new ChannelMask()
                , frequency
                , frequency);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Hand"))
            return;

        Stoptouch();
        logTouch();
        ChannelList.Stop();
    }

    private float maxPenetration = 0.0f;

    private void Update()
    {
        if (DateTime.Now > timer)
        {
            timer = DateTime.Now.AddHours(2);
            Stoptouch();
        }
    }

    private DateTime timer;

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Hand") || !stimulate)
            return;

        timer = DateTime.Now.AddMilliseconds(500);

        //check distance
        //add decrease distance a bit because of collider size
        float distance = getDistanceToPoint(other.transform.position);

        // study logging
        float penetration = maxDistance - distance;
        maxPenetration = Mathf.Max(penetration, maxPenetration);

        // convert to absolute distance
        distance = Mathf.Abs(distance);

        int pulseWidth1 = (int)Mathf.Round(getPulseWidth(distance, maxDistance, maxPulseWidth1));
        int pulseWidth2 = (int)Mathf.Round(getPulseWidth(distance, maxDistance, maxPulseWidth2));

        //print(distance + "|" + maxDistance + ":" + pulseWidth1 + "|" + pulseWidth2);
        //apply stimulation

        // update config but don't send message yet
        ChannelList.UpdateChannel(channelNumber1, new UpdateInfo(0, pulseWidth1, pulseCurrent1), false);
        // update & send message
        ChannelList.UpdateChannel(channelNumber2, new UpdateInfo(0, pulseWidth2, pulseCurrent2));
    }

    private float getDistanceToPoint(Vector3 point)
    {
        //check distance
        Plane plane = new Plane();
        plane.SetNormalAndPosition(transform.up, transform.position);

        return plane.GetDistanceToPoint(point);
    }

    private float getPulseWidth(float distance, float maxDistance, float maxValue)
    {
        float factor;

        switch (mode)
        {
            case scalingMode.linear:
                factor = getLinearScaling(distance, maxDistance);
                break;

            case scalingMode.quadratic:
                factor = getQuadraticScaling(distance, maxDistance);
                break;

            default:
                return 0;
        }

        return Mathf.Max(0, factor * maxValue + offset);
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
        return Mathf.Min(1, 1 - Mathf.Pow((distance / maxDistance), power));
    }

    private void logTouch()
    {
        SwitchWalls parent = transform.GetComponentInParent<SwitchWalls>();

        string message =
                "Registered Touch:"
                + parent.gameObject.name
                + ", MaxDistance: " + maxDistance
                + ", Penetration: " + maxPenetration
                ;
        Logger.LogLine(message);
        //print("Logged Penetration :" + message);
    }
}