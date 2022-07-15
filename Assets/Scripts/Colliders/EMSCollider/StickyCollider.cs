using UnityEngine;
using System.Collections;

public abstract class StickyCollider : MonoBehaviour
{
    public GameObject trackingObject;
    protected float maxDistance;
    protected bool tracking = false;

    protected virtual void Start()
    {
    }

    protected virtual bool OnTriggerEnter(Collider other)
    {
        if (!checkCollider(other)
           || trackingObject)
            return false;

        trackingObject = other.gameObject;

        tracking = true;
        
        return true;
    }

    protected virtual void Update()
    {
        if (trackingObject
            && !tracking
            && checkDistance())
        {
            ResetCollider();
        }
    }

    protected virtual bool OnTriggerExit(Collider other)
    {
        if (!checkCollider(other))
            return false;

        maxDistance = Vector3.Distance(transform.position, other.transform.position);

        tracking = false;

        return true;
    }

    protected virtual bool checkDistance()
    {
        return true;
        //float distance = Vector3.Distance(transform.position, trackingObject.transform.position);

        //return distance > 1.2f * maxDistance;
    }

    protected virtual bool checkCollider(Collider other)
    {
        return other.gameObject.CompareTag("HandL")
            || other.gameObject.CompareTag("HandR")
        ;
    }
    
    protected virtual Side getSide()
    {
        return trackingObject.CompareTag("HandL") ? Side.Left : Side.Right;
    }

    protected virtual void ResetCollider()
    {
        trackingObject = null;
    }
}