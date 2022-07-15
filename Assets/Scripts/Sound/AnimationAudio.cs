using UnityEngine;

public class AnimationAudio : MonoBehaviour
{
    public AudioSource[] soundClips;

    public void playSound(int index)
    {
        this.soundClips[index].Play();
    }
}