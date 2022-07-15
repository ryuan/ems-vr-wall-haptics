using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Pipe : MonoBehaviour
{
    public GameObject slider;
    public float audioThreshold = .1f;

    private Vector3 startPos;
    private Vector3 sliderStartPos;
    private Vector3 offset;

    // Use this for initialization
    private void Start()
    {
        startPos = transform.localPosition;
        sliderStartPos = slider.transform.localPosition;
        offset = startPos - sliderStartPos;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 newPos = slider.transform.localPosition + offset;

        if(Vector3.Distance(newPos, transform.localPosition) > audioThreshold)
        {
            if(!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().Play();
        }
        else
        {
            GetComponent<AudioSource>().Stop();
        }

        transform.localPosition = newPos;
    }
}