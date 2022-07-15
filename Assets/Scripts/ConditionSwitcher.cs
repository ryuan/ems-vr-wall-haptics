using UnityEngine;

public class ConditionSwitcher : MonoBehaviour
{
    public GameObject[] conditions;
    public int currentIndex = 0;
    private int lastIndex = 0;

    // Use this for initialization
    private void Start()
    {
        changeCondition(currentIndex);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnValidate()
    {
        //check bounds

        currentIndex = Mathf.Clamp(currentIndex, 0, conditions.Length - 1);

        if (lastIndex != currentIndex)
        {
            changeCondition(currentIndex);
            lastIndex = currentIndex;
        }
    }

    private void changeCondition(int index)
    {
        int i = 0;
        //disable all other gameObjects
        foreach (GameObject condition in conditions)
        {
            bool value = false;
            if (i == index)
            {
                value = true;
            }
            condition.SetActive(value);
            i++;
        }
    }
}