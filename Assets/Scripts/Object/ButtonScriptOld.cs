using UnityEngine;

public class ButtonScriptOld : MonoBehaviour
{
    public float speed = 0.5f;
    public int delay = 500;
    private Vector3 startPos;
    private Vector3 childStartPos;
    public ButtonReceiver receiver;
    private bool pushing;
    private bool pushed;
    private float timer;
    private Transform child;
    public bool pressed = false;
    public float maxPushDepth = 1.5f;

    // Use this for initialization
    private void Start()
    {
        startPos = transform.position;
        child = transform.GetChild(0);
        childStartPos = child.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        //if (pushing)
        //{
        //    if (Vector3.Distance(startPos, transform.position) < .2f)
        //    {
        //        transform.Translate(-Vector3.forward * speed * Time.deltaTime);
        //    }
        //    else if (!pushed)
        //    {
        //        pushed = true;
        //        string args = "";
        //        receiver.onPress(args);
        //        GetComponent<AudioSource>().Play();
        //    }
        //}
        //else
        //{
        if (timer <= 0)
        {
            if (Vector3.Distance(childStartPos, child.localPosition) > 0)
            {
                Vector3 targetPos = Vector3.MoveTowards(child.localPosition, childStartPos, speed * Time.deltaTime);
                if (targetPos.z < handZ)
                {
                    child.localPosition = targetPos;
                }
                else if (Mathf.Abs(targetPos.z - handZ) < 0.05f)
                {
                    lastZ = handZ;
                }
            }
            //pushed = false;
        }
        else
        {
            timer -= Time.deltaTime;
            if (pushing && lastZ <= childStartPos.z)
            {
                if (lastZ > -maxPushDepth)
                {
                    child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, lastZ);
                }
                else if (!pressed)
                {
                    GetComponent<AudioSource>().Play();
                    receiver.onPress("");
                    pressed = true;
                }
            }
        }
        //}
        // update posuition
    }

    private float startZ;
    private float lastZ;
    private float handZ;
    //private Vector3

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandL")
           || other.gameObject.CompareTag("HandR"))
        {
            //pushing = true;
            //timer = delay / 1000;
            //print("pushing");
            startZ = transform.InverseTransformPoint(other.transform.position).z;
            //child.position = new Vector3(child.position.x, child.position.y, other.transform.position.z);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("HandL")
            || other.gameObject.CompareTag("HandR"))
        {
            float z = transform.InverseTransformPoint(other.transform.position).z - startZ;
            handZ = z;
            if (z <= lastZ)
            {
                pushing = true;
                lastZ = z;
                timer = delay / 1000.0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("HandL")
            || other.gameObject.CompareTag("HandR"))
        {
            pushing = false;
            lastZ = childStartPos.z;
            pressed = false;
        }
    }
}