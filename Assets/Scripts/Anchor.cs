using UnityEngine;

public class Anchor : MonoBehaviour
{
    public float initSpeed = 1;
    public float speed = 1;
    public float rotSpeed = 1;
    public bool moveTransform = true;
    public GameObject target;
    public GameObject target1;

    // Use this for initialization
    public void Start()
    {
        speed = initSpeed;
    }

    public void OnValidate()
    {
        if (!target1)
        {
            //print("no target1 in " + name);
            return;
        }
        if (!target)
        {
            //print("no target in " + name);
            return;
        }
        UpdatePos();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdatePos();
    }

    private void UpdatePos()
    {
        if (!moveTransform)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target.transform.position, speed);
            GetComponent<Rigidbody>().MovePosition(newPos);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, rotSpeed);
            GetComponent<Rigidbody>().MoveRotation(rotation);
        }
        else
        {
            transform.position = target1.transform.position;
            transform.localRotation = target1.transform.rotation;
        }
    }
}