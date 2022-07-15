using System.Collections;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public SwitchRooms roomSwitcher;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HMD"))
        {
            roomSwitcher.index = 2;
        }
    }
}