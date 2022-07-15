using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float speed = 1.0f;
    public Space space = Space.World;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0, space);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(speed * Time.deltaTime, 0, 0, space);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, -speed * Time.deltaTime, space);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, speed * Time.deltaTime, space);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(0, -speed * Time.deltaTime, 0, space);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(0, speed * Time.deltaTime, 0, space);
        }
    }
}