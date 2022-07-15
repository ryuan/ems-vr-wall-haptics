using UnityEngine;

public class VelocityTracker : MonoBehaviour
{
    public Vector3 velocity;
    public int updateInterval = 100;
    private float timer = 0;
    public float magnitude;
    private Vector3 lastPosition;

    // Use this for initialization
    private void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime * 1000;
            return;
        }
        velocity = transform.position - lastPosition;
        magnitude = velocity.magnitude;
        lastPosition = transform.position;
        timer = updateInterval;
    }
}