using UnityEngine;
using System;

namespace MuscleDeck
{
    [RequireComponent(typeof(AudioSource))]
	public class ElectroWallCollider : RepulsionCollider
    {
        [Header("Visual Stuff")]
        protected AudioSource shockSound;

        protected Animation handAnimationL;
        protected Animation handAnimationR;

        protected override void Start()
        {
            base.Start();

            shockSound = GetComponent<AudioSource>();
            handAnimationL = GameObject.Find("ModelL").GetComponent<Animation>();
            handAnimationR = GameObject.Find("ModelR").GetComponent<Animation>();
        }

        protected override void ApplyRepulsion()
        {
            base.ApplyRepulsion();
            playAnimation(trackingObject.gameObject);
        }

        protected override MessageType getMessageType()
        {
            return MessageType.ElectroWall;
        }

        protected void playAnimation(GameObject hand)
        {
            if (hand.CompareTag("HandL"))
                handAnimationL.Play();
            else
                handAnimationR.Play();
            shockSound.Play();
            FlashBang.Flash();
            return;
        }
    }
}