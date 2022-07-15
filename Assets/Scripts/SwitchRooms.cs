using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchRooms : MonoBehaviour
{
    public static SwitchRooms instance;
    public List<GameObject> rooms;
    public int index;

    [Header("Don't change this manually")]
    public int lastIndex;

    // Use this for initialization
    private void Start()
    {
        instance = this;
    }

    public void SwitchRoom(int newIndex)
    {
        List<GameObject> temp = new List<GameObject>();
        int x = 10;
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Room"))
            {
                child.transform.localPosition = new Vector3(x, 0, 0);
                x += 10;

                print(child.name);
                print(child.transform.localPosition);
                temp.Add(child.gameObject);
            }
        }
        rooms = temp;
        index = Mathf.Clamp(newIndex, 0, rooms.Count - 1);

        rooms[index].transform.localPosition = new Vector3(0, 0, 0);
        print(rooms[index].name);
        print(rooms[index].transform.localPosition);
        lastIndex = index;
        AmbientSoundSwitcher.switchSound(rooms[index].name);
        print("changed Room");
    }

    private void OnValidate()
    {
        if (index != lastIndex)
        {
            SwitchRoom(index);
        }
    }
}