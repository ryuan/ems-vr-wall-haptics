using System.Collections;
using UnityEngine;

public class CubePhysics : MonoBehaviour
{
    public KeyCode resetButton = KeyCode.Keypad1;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(resetButton))
        {
            transform.localPosition = startPosition;
            transform.localRotation = startRotation;
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = new Vector3();
            rigidbody.angularVelocity = new Vector3();
        }
    }
}