using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class PumpReceiver : ButtonReceiver
{
    public int pumpsRequired = 3;
    public int pumps = 0;

    public override void onPress(string args)
    {
        pumps++;
        GetComponent<AudioSource>().Play();
        if (pumps >= pumpsRequired)
        {
            print("pumping rdy");
            GetComponent<ButtonController>().ChangeState(true);
        }
    }

    public override void Reset()
    {
        pumps = 0;
        GetComponent<ButtonController>().ChangeState(false);
    }
}