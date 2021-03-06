using UnityEngine;

public class ButtonScript : StickyCollider
{
    public float speed = 0.5f;
    public float minPenetration = 0.0f;
    public float maxPenetration = 0.2f;
    public ButtonReceiver receiver;
    protected bool pressed = false;
    protected Transform child;
    protected Vector3 childStartPos;
    //protected float startZ;

    protected override void Start()
    {
        child = transform.GetChild(0).GetChild(0);
        childStartPos = child.localPosition;
    }

    protected override void Update()
    {
        base.Update();

        if (trackingObject)
        {
            // penetration
            float z = transform.InverseTransformPoint(trackingObject.transform.position).z;

            if (z < maxPenetration)
            {
                if (z > minPenetration)
                    child.localPosition = new Vector3(
                        child.localPosition.x,
                        child.localPosition.y,
                        z- minPenetration);
            }
            else if (!pressed)
            {
                OnPressed();
            }
        }
        else if (Vector3.Distance(childStartPos, child.localPosition) > 0)
        {
            Vector3 targetPos = Vector3.MoveTowards(child.localPosition, childStartPos, speed * Time.deltaTime);
            child.localPosition = targetPos;
        }
        else
        {
            pressed = false;
        }
    }

    protected virtual void OnPressed()
    {
        GetComponent<AudioSource>().Play();
        receiver.onPress("");
        pressed = true;
    }

    protected override bool OnTriggerEnter(Collider other)
    {
        if (!base.OnTriggerEnter(other))
            return false;

        //startZ = transform.InverseTransformPoint(other.transform.position).z;
        //startZ = transform.lossyScale.z * .5f;

        return true;
    }

    protected override bool checkDistance()
    {
        float distance = Vector3.Distance(transform.position, trackingObject.transform.position);

        return distance > maxDistance;
    }
}