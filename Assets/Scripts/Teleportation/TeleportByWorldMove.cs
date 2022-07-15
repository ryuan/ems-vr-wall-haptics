using UnityEngine;
using System.Collections;

public class TeleportByWorldMove : MonoBehaviour {
    public bool canTeleport = true;
    public TeleportByWorldMove otherGate;
    public GameObject rootNodeToBeMoved;
    private GameObject lastTeleportedObject;

    private void OnTriggerEnter(Collider collider)
    {
        // Ignore everything if we can't teleport
        if (!canTeleport)
        {
            return;
        }

        // Don't teleport again if we just teleported,
        // Only react to camera
        if (!(collider.gameObject.CompareTag("HMD") && lastTeleportedObject == null))
        {
            return;
        }

        // Get the camera's position in local space
        var localCameraPos = transform.InverseTransformPoint(collider.transform.position);

        // Calculate the required rotation - ideally, both portals are "back to back" (rotated 180 degrees)
        // in that case we do not need to rotate the scene. In all other cases we need to modify the scene
        // depending on the relative rotation of the portals [1]
        var ownRotation = transform.rotation;

        // Tell the other gate to perform the teleportation
        otherGate.TeleportToSelf(localCameraPos, ownRotation, collider);
    }

    private void OnTriggerLeave(Collider collider)
    {
        // If the target leaves the teleport area, we can teleport it again once it enters
        // The volume again
        if (lastTeleportedObject == collider.gameObject)
        {
            lastTeleportedObject = null;
        }
    }

    public void TeleportToSelf(Vector3 localCoordinates, Quaternion otherPortalRotation, Collider target)
    {
        // [1] Determine required rotation
        var rotate180Deg = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        var idealExitRotation = otherPortalRotation * rotate180Deg;
        var requiredDeltaRotation = Quaternion.Inverse(transform.rotation) * idealExitRotation;
        rootNodeToBeMoved.transform.rotation = rootNodeToBeMoved.transform.rotation * requiredDeltaRotation;

        // Calculate the world position of the local coords
        var teleportedWorldPosition = transform.TransformPoint(localCoordinates);

        // Determine how much we have to move the world
        var worldMovement = target.transform.position - teleportedWorldPosition;

        // Teleport
        rootNodeToBeMoved.transform.position = rootNodeToBeMoved.transform.position + worldMovement;
        lastTeleportedObject = target.gameObject;
    }

    // Returns true if the object or any parent has the given name
    private bool anyParentHasName(string name, GameObject gameObject)
    {
        if (gameObject.name == name)
        {
            return true;
        } else
        {
            if (gameObject.transform.parent != null)
            {
                return anyParentHasName(name, gameObject.transform.parent.gameObject);
            } else
            {
                return false;
            }
        }
 
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
