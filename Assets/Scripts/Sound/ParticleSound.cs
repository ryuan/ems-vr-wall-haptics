using UnityEngine;

public class ParticleSound : MonoBehaviour
{
    public new ParticleSystem particleSystem;
    public AudioSource[] particleSounds;
    //public int soundCount = 2;

    // Use this for initialization
    private void Start()
    {
    }

    private int lastParticleCount = 0;

    // Update is called once per frame
    private void Update()
    {
        if (particleSystem.particleCount > lastParticleCount)
        {
            int i = Random.Range(0, particleSounds.Length);
            particleSounds[i].Play();
        }

        lastParticleCount = particleSystem.particleCount;
    }
}