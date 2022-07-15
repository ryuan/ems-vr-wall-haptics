using UnityEngine;

public class AmbientSoundSwitcher : MonoBehaviour
{
    public GameObject currentSound;
    private static AmbientSoundSwitcher instance;

    public static void switchSound(string name)
    {
        instance.switchSoundTo(name);
    }

    private void switchSoundTo(string name)
    {
        if (currentSound.name != name)
        {
            currentSound.GetComponent<AudioSource>().Stop();
            foreach (Transform child in transform)
            {
                if (child.name == name)
                {
                    currentSound = child.gameObject;
                    currentSound.GetComponent<AudioSource>().Play();
                    break;
                }
            }
        }
    }

    // Use this for initialization
    private void Start()
    {
        instance = this;
        currentSound.GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}