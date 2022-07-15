using UnityEngine;
using System;

public class ElectroBallFactory : MonoBehaviour
{
    public GameObject prefab;
    public GameObject ball;
    public AudioSource shootSound;

    public int cooldown = 2000;
    private DateTime activeTime = DateTime.Now;

    // Use this for initialization
    private void Start()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HMD") && activeTime < DateTime.Now)
        // if (activeTime < DateTime.Now)
        {
            ShootBall();
        }
    }

    private void ShootBall()
    {
        ball = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
        ball.transform.parent = transform.parent;
        ball.SendMessage("init");
        shootSound.Play();

        activeTime = DateTime.Now.AddMilliseconds(cooldown);
    }
}