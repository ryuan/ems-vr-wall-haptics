using UnityEngine;

public class ElectroBall : MonoBehaviour
{
    [Header("Wrist")]
    public bool useWrist;

    [Tooltip("max Value in mA")]
    public int wristCurrent = 15;

    [Tooltip("in ms")]
    public int wristWidthL = 100;

    [Tooltip("in ms")]
    public int wristWidthR = 100;

    [Header("Biceps")]
    public bool useBiceps;

    [Tooltip("max Value in mA")]
    public int bicepsCurrent = 15;

    [Tooltip("in ms")]
    public int bicepsWidthL = 100;

    [Tooltip("in ms")]
    public int bicepsWidthR = 100;

    [Header("Shoulder")]
    public bool useShoulder;

    [Tooltip("max Value in mA")]
    public int shoulderCurrent = 15;

    [Tooltip("in ms")]
    public int shoulderWidthL = 100;

    [Tooltip("in ms")]
    public int shoulderWidthR = 100;

    [Header("Other Settings")]
    public float speed = 1f;

    public AudioSource impactHandSound;
    public AudioSource impactFloorSound;

    private bool targeting;
    private GameObject _target;
    private Vector3 startPos;
    private bool dying;
    private bool stimulate = true;

    // Use this for initialization

    private void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (targeting)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            Vector3 pos = Vector3.MoveTowards(transform.position, _target.transform.position, speed * Time.fixedDeltaTime);
            rigidbody.MovePosition(pos);
            //transform.position = pos;
            //print(pos);
            if (Vector3.Distance(transform.position, _target.transform.position) < 0.01f)
            {
                stimulate = false;
                onHit();
            }
        }
        if (dying && Vector3.Distance(startPos, transform.position) > 1.0f)
        {
            Destroy(gameObject);
        }
    }

    private Vector3 hitVector = new Vector3(0, 0, 0);

    private void onHit()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        targeting = false;
        rigidbody.isKinematic = false;
        rigidbody.AddForce(hitVector);
        rigidbody.useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandL"))
        {
            if (!stimulate)
                return;
            hitVector = transform.position - other.transform.position;
            hitVector.Normalize();
            hitVector *= 10f;
            onHit();
            impactHandSound.Play();
            FlashBang.Flash();
            Stimulate(true);
        }
        else if (other.gameObject.CompareTag("HandR"))
        {
            if (!stimulate)
                return;
            hitVector = transform.position - other.transform.position;
            hitVector.Normalize();
            hitVector *= 10f;
            onHit();
            impactHandSound.Play();
            FlashBang.Flash();
            Stimulate(false);
        }
        else if (other.gameObject.CompareTag("Floor"))
        {
            impactFloorSound.Play();
            FlashBang.Flash();
            dying = true;
            startPos = transform.position;
        }
    }

    private void Stimulate(bool isLeft)
    {
        Side side = isLeft ?
                    Side.Left
                     : Side.Right;

        if (useWrist)
        {
            int pulseWidth1 = isLeft ? wristWidthL : wristWidthR;
            ArmStimulation.StimulateArmSinglePulse(Part.Wrist, side, pulseWidth1, wristCurrent);
        }
        if (useBiceps)
        {
            int pulseWidth2 = isLeft ? bicepsWidthL : bicepsWidthR;
            ArmStimulation.StimulateArmSinglePulse(Part.Biceps, side, pulseWidth2, bicepsCurrent);
            //print(pulseWidth2);
        }
        if (useShoulder)
        {
            int pulseWidth3 = isLeft ? shoulderWidthL : shoulderWidthR;
            ArmStimulation.StimulateArmSinglePulse(Part.Shoulder, side, pulseWidth3, shoulderCurrent);
        }
    }

    public void init()
    {
        _target = GameObject.Find("Camera");
        targeting = true;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddTorque(new Vector3(0, 1, 1), ForceMode.VelocityChange);
    }
}