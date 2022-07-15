using UnityEngine;

public class ResetHMDDrift : MonoBehaviour
{
    public KeyCode key = KeyCode.R;
    public new GameObject camera;

    private Transform init;

    // Use this for initialization
    private void Start()
    {
        init = gameObject.transform;
        reset();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            reset();
        }
    }

    private void reset()
    {
        if (camera != null)
        {
            gameObject.transform.localEulerAngles = new Vector3(0, -camera.transform.localEulerAngles.y, 0);
        }
    }
}