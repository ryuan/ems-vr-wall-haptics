using UnityEngine;

/**
 * Compensate for the fact that the native Oculus Integration always tracks position.
 */

public class CompensateOVRTracking : MonoBehaviour
{
    public new GameObject camera;
    // Use this for initialization

    // Update is called once per frame
    private void Update()
    {
        transform.localPosition = -camera.transform.localPosition;
    }
}