using System;
using UnityEngine;

public class ElectroWall : MonoBehaviour
{
    public int timeout = 500;

    [Header("EMS  Settings")]
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

    private DateTime lastShock;

    private UpdateInfo config;

    [Header("Visual Stuff")]
    public AudioSource shockSound;

    private GameObject hand;

    public Animation handAnimationL;
    public Animation handAnimationR;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!
    //         (other.gameObject.CompareTag("Hand")
    //         || other.gameObject.CompareTag("HandR")
    //         || other.gameObject.CompareTag("HandL")
    //         )
    //     )
    //        return;

    //}

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

        //print(other.name);
        if ((DateTime.Now - lastShock).Milliseconds >= timeout)
        {
            hand = other.gameObject;
            StartStimulation();
            lastShock = DateTime.Now;
        }
    }

    public void StartStimulation()
    {
        // play animation
        playAnimation(hand);

        // Shock user
        Side side = hand.CompareTag("HandL") ?
               Side.Left
                : Side.Right;
        if (useWrist)
        {
            int pulseWidth1 = hand.CompareTag("HandL") ? wristWidthL : wristWidthR;
            ArmStimulation.StimulateArmSinglePulse(Part.Wrist, side, pulseWidth1, wristCurrent);
        }

        if (useBiceps)
        {
            int pulseWidth2 = hand.CompareTag("HandL") ? bicepsWidthL : bicepsWidthR;
            ArmStimulation.StimulateArmSinglePulse(Part.Biceps, side, pulseWidth2, bicepsCurrent);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (!
    //        (other.gameObject.CompareTag("Hand")
    //        || other.gameObject.CompareTag("HandR")
    //        || other.gameObject.CompareTag("HandL")
    //        )
    //    )
    //        return;

    //    //logTouch();
    //}

    private void playAnimation(GameObject hand)
    {
        if (hand.CompareTag("HandL"))
            handAnimationL.Play();
        else
            handAnimationR.Play();
        shockSound.Play();
        FlashBang.Flash();
        return;
    }
}