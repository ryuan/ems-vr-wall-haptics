using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public string soundName;

    // Use this for initialization
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("HMD"))
            return;

        AmbientSoundSwitcher.switchSound(soundName);
    }
}