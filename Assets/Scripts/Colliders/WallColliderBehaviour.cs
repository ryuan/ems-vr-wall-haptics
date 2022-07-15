using System.Threading;
using UnityEngine;

public class WallColliderBehaviour : PersistBehaviour
{
    public int channelFrequency = 100;
    public int timeBeforeHold = 100; // time in ms before hold values are applied
    public Channels channel = Channels.Channel1; //for wrist
    public int initPulseWidth = 100;
    public int initPulseCurrent = 25;
    public int holdPulseWidth = 75;
    public int holdPulseCurrent = 25;
    public Channels channel1 = Channels.Channel2; //for shoulder
    public int initPulseWidth1 = 100;
    public int initPulseCurrent1 = 25;
    public int holdPulseWidth1 = 75;
    public int holdPulseCurrent1 = 25;
    //public KeyCode testKey = KeyCode.T;

    public void Update()
    {
        //if (Input.GetKeyDown(testKey))
        //{
        //    startTouch();
        //}
        //else if (Input.GetKeyUp(testKey))
        //{
        //    stopTouch();
        //}
    }

    private Thread touchThread;
    private System.Diagnostics.Stopwatch stopwatch;
    private bool isReady = false;
    private bool isRunning = false;

    private void OnTriggerEnter(Collider other)
    {
        //check if its the correct object
        if (!other.gameObject.CompareTag("Hand") || isRunning || !isReady)
            return;

        startTouch();
    }

    // use stopwatch in a separate thread, because using Thread.Sleep() in Unity caps to 64 Hz
    private void startTouch()
    {
        try
        {
            //init RehaStimDevice
            ChannelMask channels = new ChannelMask();
            channels.setChannel((int)channel);
            channels.setChannel((int)channel1);

            ChannelList.InitCM(
                    0
                    , channels
                    , new ChannelMask()
                    , 300 // we're not using this
                    , channelFrequency);

            // define the thread and assign function for thread loop
            touchThread = new Thread(new ThreadStart(touchProcess));
            // Boolean used to determine the thread is running

            isRunning = true;

            touchThread.Priority = System.Threading.ThreadPriority.Highest;

            stopwatch = new System.Diagnostics.Stopwatch();
            // Start the thread
            touchThread.Start();
            //Debug.Log("Pulse thread started");
        }
        catch (System.Exception ex)
        {
            // Failed to start thread
            Debug.Log("Error 3: " + ex.Message.ToString());
        }
    }

    /**
     * Immediately send a strong EMS pulse to get to the desired position,
     * then lower the intensity to more or less hold it.
     */

    private void touchProcess()
    {
        stopwatch.Start();

        //Debug.Log("Boosting");#

        //wrist
        UpdateInfo init = new UpdateInfo(UpdateInfo.MODE_SINGLE, initPulseWidth, initPulseCurrent);
        ChannelList.UpdateChannel((int)channel, init, false);

        //shoulder
        UpdateInfo init1 = new UpdateInfo(UpdateInfo.MODE_SINGLE, initPulseWidth1, initPulseCurrent1);
        ChannelList.UpdateChannel((int)channel1, init1);
        // wait a bit before decreasing intensity
        while (stopwatch.ElapsedMilliseconds < timeBeforeHold) { }

        //Debug.Log("Holding");

        //hold lower intensity
        //wrist
        UpdateInfo hold = new UpdateInfo(UpdateInfo.MODE_SINGLE, holdPulseWidth, holdPulseCurrent);
        ChannelList.UpdateChannel((int)channel, hold, false);

        //shoulder
        UpdateInfo hold1 = new UpdateInfo(UpdateInfo.MODE_SINGLE, holdPulseWidth1, holdPulseCurrent1);
        ChannelList.UpdateChannel((int)channel1, hold1);

        //wait for Trigger Exit
        while (isRunning) { }

        // ramp down again
        stopwatch.Reset();
        stopwatch.Start();

        int stop = Mathf.Max(holdPulseWidth, holdPulseWidth1);
        for (int i = 0; i < stop; i += 5)
        {
            int width = Mathf.Max(0, holdPulseWidth - i);
            UpdateInfo rampDown = new UpdateInfo(UpdateInfo.MODE_SINGLE, width, holdPulseCurrent);
            ChannelList.UpdateChannel((int)channel, rampDown);

            int width1 = Mathf.Max(0, holdPulseWidth1 - i);
            UpdateInfo rampDown1 = new UpdateInfo(UpdateInfo.MODE_SINGLE, width1, holdPulseCurrent);
            ChannelList.UpdateChannel((int)channel1, rampDown1);

            // wait a bit
            while (stopwatch.ElapsedMilliseconds < 10)
            {
            }

            stopwatch.Reset();
            stopwatch.Start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //check if its the correct object
        if (!other.gameObject.CompareTag("Hand"))
            return;

        //stopTouch();
    }

    private void stopTouch(bool force = false)
    {
        if (touchThread != null)
        {
            // start ramping down
            isRunning = false;
            // give thread some time to finish
            if (force)
                touchThread.Abort();
            touchThread = null;
        }

        ChannelList.Stop();

        if (stopwatch != null)
            stopwatch.Stop();

        //Debug.Log("Exiting");
    }

    private void OnDestroy()
    {
        Thread.Sleep(100);
        stopTouch();
    }

    private void OnApplicationQuit()
    {
        Thread.Sleep(100);
        stopTouch();
    }

    public void setReady(bool value)
    {
        isReady = value;
    }
}