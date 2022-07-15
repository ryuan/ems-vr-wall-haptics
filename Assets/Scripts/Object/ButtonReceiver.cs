using UnityEngine;

public abstract class ButtonReceiver : Resetable
{
    public abstract void onPress(string args);
}