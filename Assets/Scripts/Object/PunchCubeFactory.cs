using UnityEngine;

public class PunchCubeFactory : MonoBehaviour
{
    public GameObject prefab;
    public GameObject cube;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (cube == null)
        {
            cube = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
            cube.transform.parent = transform;
        }
    }
}