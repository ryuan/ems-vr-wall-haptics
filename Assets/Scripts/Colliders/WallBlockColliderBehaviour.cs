using UnityEngine;

public class WallBlockColliderBehaviour : MonoBehaviour
{
    [Header("EMS  Settings")]
    public int pulseCount = 5;

    [Header("Channel1")]
    public Channels channelNumber1 = Channels.Channel1;

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

    [Header("Visual Stuff")]
    public Animator animator;

    public Animation handAnimation;

    public int vibroDuration = 100;

    // Use this for initialization
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hand"))
            return;

        bump();
    }

    public void bump()
    {
        //notify parent instead, so no multiple shocks for each block?

        //play animation
        animator.SetTrigger("Bump");
        handAnimation.Play();

        // Shock user
        for (int i = 0; i < pulseCount; i++)
        {
            SinglePulse.sendSinglePulse(channelNumber1, pulseWidth1, pulseCurrent1);
        }
        for (int i = 0; i < pulseCount; i++)
        {
            SinglePulse.sendSinglePulse(channelNumber2, pulseWidth2, pulseCurrent2);
        }

        //Send Arduino Signal
        SerialInterface.SendMessage(vibroDuration.ToString());
    }
}