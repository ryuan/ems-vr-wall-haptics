using UnityEngine;

public class FollowScript : PersistBehaviour
{
    public GameObject target;
    public bool followPosition = true;
    public bool followRotation = true;
    public bool applyToLocal = true;
    public float xOffset = 0.0f;
    public float yOffset = 0.0f;
    public float zOffset = 0.0f;

    private void OnValidate()
    {
        if (!target)
        {
            //print("no target1 in " + name);
            return;
        }
        UpdatePosition();
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        // follow the target
        if (followPosition)
        {
            Vector3 posOffset = new Vector3(xOffset, yOffset, zOffset);
            if (applyToLocal)
                transform.localPosition = target.transform.position + posOffset;
            else
                transform.position = target.transform.position + posOffset;
        }
        if (followRotation)
        {
            if (applyToLocal)
                transform.localRotation = target.transform.rotation;
            else
                transform.rotation = target.transform.rotation;
        }
    }
}