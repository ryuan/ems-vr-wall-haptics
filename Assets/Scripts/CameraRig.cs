using System.Collections;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [HideInInspector]
    public bool Tracked;

    private GameObject _target;
    private bool _isCalibrated;
    private const float TimeForCalibration = 3.0f;
    private float _calibratingTime;
    private const float AngleOffsetThreshold = 3.0f;
    private float _angleOffsetJumpBack = 3.0f;
    private Quaternion _angleAxisAddRot;
    private bool _followVideoCamera;
    public Transform EditorCamera;
    public bool TrackEditorCameraInPlayMode;
    public float FieldOfViewEditorCamera = 47.0f;
    public GameObject TrackingIndicator;

    public enum VideoSettings
    {
        LowMobile,
        HighMobile,
        EditorPreview
    }

    public void Awake()
    {
        Tracked = true;
        _followVideoCamera = TrackEditorCameraInPlayMode && Application.isEditor && EditorCamera != null;
        //if shooting for video capture, follow video camera
        if (_followVideoCamera)
        {
            Camera.main.transform.parent = EditorCamera;
            Camera.main.transform.localRotation = Quaternion.identity;
            Camera.main.fieldOfView = FieldOfViewEditorCamera;
        }
        else //else apply main cam to this
        {
            //Camera.main.transform.parent = transform;
        }
        //Camera.main.transform.localPosition = Vector3.zero;
    }

    public void Start()
    {
        _target = GameObject.Find("cv1");
        StartCoroutine(Following(_target));
    }

    public IEnumerator StartFollowing(string targetName, Quaternion addRot)
    {
        if (_followVideoCamera) yield break;
        _angleAxisAddRot = addRot;
        StopAllCoroutines();
        yield return StartCoroutine(FindTarget(targetName));
        yield return StartCoroutine(Following(_target));
    }

    private IEnumerator Following(GameObject target)
    {
        while (true)
        {
            yield return null;
            transform.localPosition = target.transform.position;
            Tracked = target.tag != "untracked";
            if (target.tag != "untracked")
            {
                if (_followVideoCamera)
                {
                    transform.rotation = target.transform.rotation * _angleAxisAddRot;//since cam doesn't move

                    var cam = transform.GetChild(0);
                    cam.localRotation = Quaternion.identity;
                }
                else
                {
                    //constantly correct orientation from optitrack -- problematic with drop outs
                    Quaternion twist;
                    Quaternion swing;
                    var targetRot = target.transform.rotation;// angleAxisAddRot;// Quaternion.Euler(eulerOffset);
                    SwingTwistDecomposition(targetRot, Vector3.up, out twist, out swing);

                    Quaternion cameraTwist;
                    Quaternion cameraSwing;
                    SwingTwistDecomposition(Camera.main.transform.localRotation, Vector3.up, out cameraTwist, out cameraSwing);

                    var offset = Quaternion.Angle(transform.rotation, twist * Quaternion.Inverse(cameraTwist));
                    //Debug.Log("cam:" + Camera.main.transform.localRotation.eulerAngles.y);
                    //Debug.Log("target:" + target.transform.rotation.eulerAngles.y);
                    if (offset > AngleOffsetThreshold)
                    {
                        _isCalibrated = false;
                        _calibratingTime = 0.0f;
                    }
                    else if (_calibratingTime < TimeForCalibration)
                    {
                        _calibratingTime += Time.deltaTime;
                    }
                    else//if (!isCalibrated && offset < angleOffsetJumpBack)
                    {
                        transform.rotation = twist * Quaternion.Inverse(cameraTwist);
                        _isCalibrated = true;
                    }

                    if (!_isCalibrated)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, twist * Quaternion.Inverse(cameraTwist), 0.01f);
                    }
                }
            }
        }
    }

    private IEnumerator FindTarget(string targetName)
    {
        _target = null;
        while (_target == null)
        {
            yield return null;
            _target = GameObject.Find(targetName);
        }
        ResetOrientation();
    }

    private void ResetOrientation()
    {
        Quaternion twist;
        Quaternion swing;
        SwingTwistDecomposition(_target.transform.rotation, Vector3.up, out twist, out swing);
        Debug.Log("tracking:" + _target.transform.rotation.eulerAngles.y);

        Quaternion cameraTwist;
        Quaternion cameraSwing;
        SwingTwistDecomposition(Camera.main.transform.localRotation, Vector3.up, out cameraTwist, out cameraSwing);
        Debug.Log("camera:" + Camera.main.transform.localRotation.eulerAngles.y);

        transform.rotation = twist * Quaternion.Inverse(cameraTwist);
        Debug.Log("rig:" + transform.rotation.eulerAngles.y);
    }

    //private static void SetTintColor(Material mat, Color c) {
    //	mat.SetColor("_TintColor", c);
    //}

    private void SwingTwistDecomposition(Quaternion q, Vector3 v, out Quaternion twist, out Quaternion swing)
    {
        var rotationAxis = new Vector3(q.x, q.y, q.z);
        var projection = Vector3.Project(rotationAxis, v);
        var magnitude = Mathf.Sqrt(Mathf.Pow(projection.x, 2) + Mathf.Pow(projection.y, 2) + Mathf.Pow(projection.z, 2) + Mathf.Pow(q.w, 2));
        twist = new Quaternion(projection.x / magnitude, projection.y / magnitude, projection.z / magnitude, q.w / magnitude);
        var twistConjugated = new Quaternion(-projection.x / magnitude, -projection.y / magnitude, -projection.z / magnitude, q.w / magnitude);
        swing = q * twistConjugated;
    }
}