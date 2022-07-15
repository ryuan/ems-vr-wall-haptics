using System.Collections;
using UnityEngine;

public class RoomReset : MonoBehaviour
{
    public Resetable[] resetObjects;
    public KeyCode resetKey;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            foreach (Resetable obj in resetObjects)
            {
                obj.Reset();
            }
        }
    }
}