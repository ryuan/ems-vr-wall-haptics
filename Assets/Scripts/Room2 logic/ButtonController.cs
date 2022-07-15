using System.Collections;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public delegate void ButtonPressDelegate(bool newPressState);

    public event ButtonPressDelegate OnPressStateChange;

    private ArrayList connectedObjects = new ArrayList();

    public bool isPressed = false;

    public AudioSource putSound;

    public AudioSource releaseSound;

    [Header("Dont change manually")]
    public bool lastState = false;

    private void OnTriggerEnter(Collider other)
    {
        // Ignore things that are not named cube
        if (other.name.ToLower().IndexOf("cube") < 0)
        {
            return;
        }

        ChangeState(true);

        connectedObjects.Add(other);
    }

    public void ChangeState(bool newState)
    {
        if (newState && !isPressed)
        {
            Debug.Log("Button '" + name + "' state changed to pressed");
            isPressed = true;
            var animation = GetComponent<Animation>();
            if (animation != null)
            {
                animation.Play("ButtonPress");
            }

            putSound.Play();

            if (OnPressStateChange != null)
            {
                OnPressStateChange(isPressed);
            }
        }
        else if (!newState && isPressed)
        {
            Debug.Log("Button '" + name + "' state changed to NOT pressed");
            isPressed = false;
            releaseSound.Play();

            var animation = GetComponent<Animation>();
            if (animation != null)
            {
                animation.Play("ButtonRelease");
            }

            if (OnPressStateChange != null)
            {
                OnPressStateChange(isPressed);
            }
        }
    }

    private void OnValidate()
    {
        if (lastState != isPressed)
        {
            if (OnPressStateChange != null)
            {
                OnPressStateChange(isPressed);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (connectedObjects.Contains(other) == false)
        {
            return;
        }

        connectedObjects.Remove(other);

        if (connectedObjects.Count == 0)
        {
            ChangeState(false);
        }
    }

    // Use this for initialization
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        isPressed = false;
        Debug.Log("ButtonController initialized");
    }
}