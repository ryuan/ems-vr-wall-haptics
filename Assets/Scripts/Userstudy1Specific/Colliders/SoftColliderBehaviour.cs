using UnityEngine;

/**
 * Collider script used in the first Userstudy for the soft walls.
 * Ramps up EMS only based on distance of the hand to this object's center.
 */

public class SoftColliderBehaviour : PersistBehaviour
{
    public Channels channel1 = Channels.Channel1;
    public Channels channel2 = Channels.Channel2;

    public int frequency = 75;
    public int current = 25;
    public GameObject rootObject;                // The containing GameObject
    //public int decreasePerTick = 5;             // How much the pulse width should be decreased per tick

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private float maxDistance; // the distance to the reference object when entering the collider

    private void OnTriggerEnter(Collider other)
    {
        //check if its the correct object
        if (!other.gameObject.CompareTag("Hand"))
            return;

        float distance = getDistanceToPlane(other.gameObject);
        // if not coming from the hard zone
        //if (distance < 0.25f)
        //    return;

        //save maxDistance
        maxDistance = 1f;

        //init EMS
        ChannelMask channels = new ChannelMask();
        channels.setChannel(channel1);
        channels.setChannel(channel2);

        ChannelList.InitCM(0
                , channels
                , new ChannelMask()
                , frequency
                , frequency);
    }

    public int maxPulseWidth = 100;
    public int maxPulseWidth2 = 200;
    public float timeout = 1;
    private float timeSinceLastUpdate = 0.0f;

    private void OnTriggerStay(Collider other)
    {
        //check if its the correct object
        if (!other.gameObject.CompareTag("Hand"))
            return;

        // update intensity
        // pulseWidth modulation
        float pulseWidth = 0.0f;
        float pulseCurrent = current;

        float distance = getDistanceToPlane(other.gameObject);

        if (distance < maxDistance)
        {
            //linear scaling
            pulseWidth = getLinearScaling(distance, maxDistance, maxPulseWidth);
            //gradually change back when coming from hard zone?
            //print("Intensity: " + pulseWidth);

            // update wrist
            //int currentPulseWidth = ChannelList.GetChannelConfig(channel1).pulseWidth;
            timeSinceLastUpdate += Time.deltaTime;
            if (timeSinceLastUpdate > timeout)
            {
                timeSinceLastUpdate = 0;
                //currentPulseWidth -= decreasePerTick;
            }

            //pulseWidth = Mathf.Max(pulseWidth, currentPulseWidth);

            // update config but don't send message yet
            ChannelList.UpdateChannel(channel1, new UpdateInfo(0, (int)pulseWidth, (int)pulseCurrent), false);

            // update shoulder
            pulseWidth = getLinearScaling(distance, maxDistance, maxPulseWidth2);
            pulseCurrent = 15.0f;
            ChannelList.UpdateChannel(channel2, new UpdateInfo(0, (int)pulseWidth, (int)pulseCurrent));
        }
    }

    private float getLinearScaling(float distance, float maxDistance, float maxValue)
    {
        return (maxDistance - distance) / maxDistance * maxValue;
    }

    private float getDistanceToPlane(GameObject other)
    {
        Plane plane = new Plane();
        plane.SetNormalAndPosition(rootObject.gameObject.transform.up, rootObject.gameObject.transform.position);
        float distance = Mathf.Abs(plane.GetDistanceToPoint(other.transform.position));

        return distance;
    }

    private void OnTriggerExit(Collider other)
    {
        //check if its the correct object
        if (!other.gameObject.CompareTag("Hand"))
            return;

        //// stop simulation if not exiting towards the hard zone
        //float distance = getDistanceToPlane(other.gameObject);
        //// return if coming from the hard zone
        //if (distance < 0.15f)
        //    return;

        ChannelList.Stop();
    }
}