using System.Collections;
using UnityEngine;

public class PumpButton : MonoBehaviour
{
    public PumpButton partner;

    public bool pressable = true;

    private float startZ;
    private float lastZ;
    private float handZ;
    private bool pushing;
    private bool pushed;

    private Vector3 startPos;
    private Vector3 childStartPos;
    private Transform partnerChild;
    private Transform child;
    public float maxPushDepth = 1.5f;
    public ButtonReceiver receiver;
    public AudioSource[] sounds;

    // Use this for initialization
    private void Start()
    {
        startPos = transform.position;
        child = transform.GetChild(0);
        partnerChild = partner.transform.GetChild(0);
        childStartPos = child.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandL")
           || other.gameObject.CompareTag("HandR"))
        {
            if (pressable)
            {
                partner.pressable = false;
            }
            else
            {
            }
        }
    }

    private void Update()
    {
        if (pushing && lastZ <= childStartPos.z)
        {
            if (lastZ > -maxPushDepth)
            {
                child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, lastZ);
                partnerChild.localPosition = new Vector3(partnerChild.localPosition.x, partnerChild.localPosition.y, -lastZ);
            }
            else
            {
                sounds[Random.Range(0, sounds.Length - 1)].Play();
                receiver.onPress("");
                partner.pressable = true;
                pressable = false;
            }
        }
        //}
        // update posuition
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("HandL")
            || other.gameObject.CompareTag("HandR"))
        {
            if (pressable)
            {
                float z = transform.InverseTransformPoint(other.transform.position).z - startZ;
                handZ = z;
                if (z <= lastZ)
                {
                    pushing = true;
                    lastZ = z;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("HandL")
           || other.gameObject.CompareTag("HandR"))
        {
            partner.pressable = true;
        }
    }
}