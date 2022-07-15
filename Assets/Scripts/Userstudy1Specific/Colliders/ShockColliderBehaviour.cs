using System;
using UnityEngine;

/**
 * Collider script used in the first userstudy for the electro wall
 */

public class ShockColliderBehaviour : MonoBehaviour
{
    [Header("EMS  Settings")]
    [Tooltip("in ms")]
    public int timeout = 1000;

    public int pulseCount = 5;

    [Header("Channel1")]
    public Channels channelNumber = Channels.Channel1;

    [Tooltip("in mA")]
    public int pulseCurrent1 = 15;

    [Tooltip("in ms")]
    public int pulseWidth1 = 100;

    [Header("Channel2")]
    public Channels channelNumber2 = Channels.Channel2;

    [Tooltip("in mA")]
    public int pulseCurrent2 = 15;

    [Tooltip("in ms")]
    public int pulseWidth2 = 100;

    private DateTime lastShock;

    private UpdateInfo config;

    [Header("Visual Stuff")]
    public GameObject thunderEmitter1;

    public GameObject thunderEmitter2;
    public GameObject ringEmitter;

    public Animation handAnimation;

    public AudioSource shockSound;

    [Header("Haptics")]
    public bool useHaptics = true;

    [Tooltip("in ms")]
    public int vibroDuration = 500;

    private float maxDistance = 1f;
    private float maxPenetration = 0.0f;
    //public GameObject hand;

    private void Update()
    {
    }

    private GameObject hand;

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

        //reset penetration
        maxPenetration = 0.0f;
    }

    private void OnTriggerStay(Collider other)
    {
        //check if its the correct object
        if (!
            (other.gameObject.CompareTag("Hand")
            || other.gameObject.CompareTag("HandR")
            || other.gameObject.CompareTag("HandL")
            )
        )
            return;

        print(other.name);
        if ((DateTime.Now - lastShock).Milliseconds >= timeout)
        {
            hand = other.gameObject;
            StartStimulation();
            lastShock = DateTime.Now;
        }

        //check distance
        //add decrease distance a bit because of collider size
        float distance = getDistanceToPoint(other.transform.position);

        // study logging
        float penetration = maxDistance - distance;
        maxPenetration = Mathf.Max(penetration, maxPenetration);
    }

    public void StartStimulation()
    {
        // play animation
        playAnimation(hand);

        // Shock user
        for (int i = 0; i < pulseCount; i++)
        {
            Side side = hand.CompareTag("HandL") ?
                   Side.Left
                    : Side.Right;

            ArmStimulation.StimulateArmSinglePulse(Part.Wrist, side, pulseWidth1, pulseCurrent1);
            ArmStimulation.StimulateArmSinglePulse(Part.Biceps, side, pulseWidth2, pulseCurrent2);

            //SinglePulse.sendSinglePulse(channelNumber, pulseWidth1, pulseCurrent1);
            //SinglePulse.sendSinglePulse(channelNumber2, pulseWidth2, pulseCurrent2);
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

        logTouch();
    }

    private void playAnimation(GameObject hand)
    {
        shockSound.Play();
        FlashBang.Flash();

        float dist1 = Vector3.Distance(hand.transform.position, thunderEmitter1.transform.position);
        ParticleSystem particleSystem1 = thunderEmitter1.GetComponent<ParticleSystem>();
        particleSystem1.startSize = dist1 * 0.33f;
        particleSystem1.Emit(1);
        float dist2 = Vector3.Distance(hand.transform.position, thunderEmitter2.transform.position);
        ParticleSystem particleSystem2 = thunderEmitter2.GetComponent<ParticleSystem>();
        particleSystem2.startSize = dist2 * 0.33f;
        particleSystem2.Emit(1);

        ringEmitter.transform.position = hand.transform.position;
        ParticleSystem particleSystem3 = ringEmitter.GetComponent<ParticleSystem>();
        particleSystem3.Emit(15);

        shockSound.Play();
        FlashBang.Flash();

        //vibration
        if (useHaptics)
        {
            SerialInterface.SendMessage(vibroDuration.ToString());
        }

        //animation
        handAnimation.Play();
    }

    private float getDistanceToPoint(Vector3 point)
    {
        //check distance
        Plane plane = new Plane();
        plane.SetNormalAndPosition(transform.up, transform.position);

        return plane.GetDistanceToPoint(point);
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