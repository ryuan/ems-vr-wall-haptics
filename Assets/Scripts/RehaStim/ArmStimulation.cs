using System.Collections;
using UnityEngine;

/**
 * Interface Class for easier access to RehaStim
 * This script should only exist once in the scene.
 */

public class ArmStimulation : MonoBehaviour
{
    public static ArmStimulation instance = null;

    public int frequency = 75;

    public int pulseCount = 5;

    [Header("Left Arm")]
    public Channels channelWristL = Channels.Channel1;

    public Channels channelBicepsL = Channels.Channel2;
    public Channels channelTricepsL = Channels.Channel3;
    public Channels channelShoulderL = Channels.Channel4;
    public Channels channelInwardsWristL = Channels.ChannelNone;
    public Channels channelOutwardsWristL = Channels.ChannelNone;

    [Header("Right Arm")]
    public Channels channelWristR = Channels.Channel5;

    public Channels channelBicepsR = Channels.Channel6;
    public Channels channelTricepsR = Channels.Channel7;
    public Channels channelShoulderR = Channels.Channel8;
    public Channels channelInwardsWristR = Channels.ChannelNone;
    public Channels channelOutwardsWristR = Channels.ChannelNone;

    [Header("Testing")]
    public int testWidth = 200;

    public int testCurrent = 15;
    public Part testPart = Part.Wrist;

    /*
     * Static call to stimulate
     */

    public static void StimulateArm(Part part, Side side, int width, int current)
    {
        if (instance)
            instance.stimulate(part, side, width, current);
    }

    public static void StimulateArmSinglePulse(Part part, Side side, int width, int current)
    {
        if (instance)
            instance.stimulateSinglePulse(part, side, width, current);
    }

    public static void StimulateArmBurst(StimulationInfo info, int duration)
    {
        if (instance)
            instance.StartCoroutine(instance.stimulateBurst(new StimulationInfo[] { info }, duration));
    }

    public static void StimulateArmBurst(StimulationInfo[] infos, int duration)
    {
        if (instance)
            instance.StartCoroutine(instance.stimulateBurst(infos, duration));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            stimulate(testPart, Side.Left, testWidth, testCurrent);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stimulate(testPart, Side.Right, testWidth, testCurrent);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Stop();
        }
    }

    // Use this for initialization
    private void Start()
    {
        instance = this;
    }

    private void Stop()
    {
        if (ChannelList.isRunning)
        {
            ChannelList.Stop();
        }
    }

    public void initStimulation()
    {
        if (!ChannelList.isRunning)
        {
            //init EMS
            ChannelMask channels = new ChannelMask();
            channels.setChannel(channelWristL);
            channels.setChannel(channelBicepsL);
            channels.setChannel(channelTricepsL);
            channels.setChannel(channelShoulderL);

            channels.setChannel(channelWristR);
            channels.setChannel(channelBicepsR);
            channels.setChannel(channelTricepsR);
            channels.setChannel(channelShoulderR);

            channels.setChannel(channelInwardsWristL);
            channels.setChannel(channelOutwardsWristL);

            channels.setChannel(channelInwardsWristR);
            channels.setChannel(channelOutwardsWristR);

            ChannelList.InitCM(0
                    , channels
                    , new ChannelMask() //low freq channels not used
                    , frequency
                    , frequency);
        }
    }

    public void stimulate(Part part, Side side, int width, int current)
    {
        //init channel mode if needed
        initStimulation();

        //update bodypart
        Channels channel = GetChannel(part, side);

        if (channel == Channels.ChannelNone)
            return;

        UpdateInfo info = new UpdateInfo();
        info.pulseCurrent = Mathf.Max(0, current);
        info.pulseWidth = Mathf.Max(0, width);

        //update EMS
        ChannelList.UpdateChannel(channel, info);
    }

    protected Channels GetChannel(Part part, Side side)
    {
        Channels channel;
        switch (part)
        {
            case (Part.Wrist):
                channel = side == Side.Left ? channelWristL : channelWristR;
                break;

            case (Part.Biceps):
                channel = side == Side.Left ? channelBicepsL : channelBicepsR;
                break;

            case (Part.Triceps):
                channel = side == Side.Left ? channelTricepsL : channelTricepsR;
                break;

            case (Part.Shoulder):
                channel = side == Side.Left ? channelShoulderL : channelShoulderR;
                break;

            case (Part.InwardsWrist):
                channel = side == Side.Left ? channelInwardsWristL : channelInwardsWristR;
                break;

            case (Part.OutwardsWrist):
                channel = side == Side.Left ? channelOutwardsWristL : channelOutwardsWristR;
                break;

            default:
                return Channels.ChannelNone;
        }

        return channel;
    }

    public void stimulateSinglePulse(Part part, Side side, int width, int current)
    {
        Stop();

        //update bodypart
        Channels channel = GetChannel(part, side);

        if (channel == Channels.ChannelNone)
            return;

        for (int i = 0; i < pulseCount; i++)
        {
            SinglePulse.sendSinglePulse(channel, width, current);
        }
    }

    public IEnumerator stimulateBurst(StimulationInfo[] infos, int duration)
    {
        System.DateTime stopTime = System.DateTime.Now.AddMilliseconds(duration);

        while (stopTime > System.DateTime.Now)
        {
            Stop();
            foreach (var info in infos)
            {
                Channels channel = GetChannel(info.part, info.side);
                SinglePulse.sendSinglePulse(channel, info.width, info.current);
            }

            yield return null;
        }
    }
}

public class StimulationInfo
{
    public Part part;
    public Side side;
    public int width;
    public int current;

    public StimulationInfo(Part part, Side side, int width, int current)
    {
        this.part = part;
        this.side = side;
        this.width = width;
        this.current = current;
    }
}

public enum Part
{
    Wrist = 1,
    Biceps,
    Triceps,
    Shoulder,
    OutwardsWrist,
    InwardsWrist,
}

public enum Side
{
    Left = 1,
    Right = 2,
}