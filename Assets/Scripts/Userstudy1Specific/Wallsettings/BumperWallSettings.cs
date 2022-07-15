using UnityEngine;

public class BumperWallSettings : PersistBehaviour
{
    [Header("Impacto Style Settings")]
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
    public Animation handAnimation;

    public int vibroDuration = 100;

    private void OnValidate()
    {
        WallBlockColliderBehaviour[] children = GetComponentsInChildren<WallBlockColliderBehaviour>(true);
        foreach (WallBlockColliderBehaviour child in children)
        {
            // apply settings to children
            child.channelNumber1 = channelNumber1;
            child.pulseCurrent1 = pulseCurrent1;
            child.pulseWidth1 = pulseWidth1;

            child.channelNumber2 = channelNumber2;
            child.pulseCurrent2 = pulseCurrent2;
            child.pulseWidth2 = pulseWidth2;

            child.handAnimation = handAnimation;
            child.vibroDuration = vibroDuration;
        }
    }
}