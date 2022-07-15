using UnityEngine;

public class GateScript : ButtonReceiver
{
    private Vector3 startPos;
    public bool opening;
    public float speed = 1.0f;
    public MuscleDeck.RoomTeleporter teleporter;

    public KeyCode toggleButton;

    protected AudioSource audioSource;

    public override void Reset()
    {
        opening = false;
        transform.localPosition = startPos;
    }

    // Use this for initialization
    private void Start()
    {
        startPos = transform.localPosition;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(toggleButton))
        {
            opening = !opening;
        }

        Vector3 targetPos = opening ? 
            startPos + new Vector3(0, 3, 0):
            startPos;

        if (transform.localPosition != targetPos)
        {
            Vector3 newPos = Vector3.MoveTowards(
                transform.localPosition,
                targetPos,
                speed * Time.deltaTime);

            transform.localPosition = newPos;

            teleporter.canTeleport = opening;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    public void Open()
    {
        if (!opening)
        {
            opening = true;
            //teleporter.canTeleport = true;
            //audioSource.Play();
        }
    }

    public void Close()
    {
        if (opening)
        {
            opening = false;
            //teleporter.canTeleport = false;
            //audioSource.Play();
        }
    }

    public override void onPress(string args)
    {
        Open();
    }
}