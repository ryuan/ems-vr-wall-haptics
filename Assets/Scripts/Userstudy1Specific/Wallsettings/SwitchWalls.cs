using System.Collections.Generic;
using UnityEngine;

public class SwitchWalls : MonoBehaviour
{
    public List<GameObject> walls;
    public int index;

    /**
     * Only have 1 wall active at a time
     */

    private void OnValidate()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name.Equals("Walls"))
            {
                List<GameObject> temp = new List<GameObject>();
                foreach (Transform wall in child)
                {
                    wall.gameObject.SetActive(false);
                    temp.Add(wall.gameObject);
                }
                walls = temp;
                index = Mathf.Clamp(index, 0, walls.Count - 1);
                walls[index].SetActive(true);
                break;
            }
        }

        /*
        foreach (GameObject wall in walls)
        {
            wall.SetActive(false);
        }

        walls[index].SetActive(true);
        */
    }
}