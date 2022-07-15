using UnityEngine;

public class WallExitColliderBehaviour : PersistBehaviour
{
    public GameObject relatedCollider;

    private void OnTriggerEnter(Collider other)
    {
        //check if its the correct object
        if (!other.gameObject.CompareTag("Hand") || relatedCollider == null)
            return;

        relatedCollider.SendMessage("setReady", true);
    }

    private void OnTriggerExit(Collider other)
    {
        //check if its the correct object
        if (!other.gameObject.CompareTag("Hand") || relatedCollider == null)
            return;

        relatedCollider.SendMessage("stopTouch", false);
        relatedCollider.SendMessage("setReady", false);
    }
}