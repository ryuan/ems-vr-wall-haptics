using UnityEngine;
using System.Collections;
using System;

public class ResetTransforms : Resetable {
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public override void Reset()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    // Use this for initialization
    void Start () {
        initialPosition = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );

        initialRotation = new Quaternion(
            transform.rotation.x,
            transform.rotation.y,
            transform.rotation.z,
            transform.rotation.w
        );
	}
}
