using UnityEngine;

public class ResetScript : MonoBehaviour
{
    private Vector3 startPosition;
    public float maxDistance = 10;

    // Use this for initialization
    private void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector3.Distance(transform.position, startPosition) > maxDistance)
        {
            //transform.position = startPosition;
            //GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            Destroy(this.gameObject);
        }
    }
}