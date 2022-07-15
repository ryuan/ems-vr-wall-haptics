using UnityEngine;

public class LookAt : MonoBehaviour
{
    public GameObject target;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (target != null)
            gameObject.transform.LookAt(target.transform);
    }
}