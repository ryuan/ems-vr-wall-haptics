using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace MuscleDeck
{
    public class VictoryScreen : RoomTeleporter
    {
        [Range(0, 1)]
        public float alpha;

        public float fadeTime;
        protected float passedTime;

        public bool doFade;
        public bool toblack;

        public Text text;
        public Image panel;

        private void OnValidate()
        {
            if (doFade && !Application.isPlaying)
            {
                doFade = false;
                ToggleFade();
            }
        }

        protected override bool OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("HMD") && canTeleport)
            {
                StartCoroutine(Fade(true));
                RehaStimInterface.instance.enableEMS = false;
                AmbientSoundSwitcher.switchSound("Paradise");
            }

            return true;
        }

        protected void OnTriggerExit(Collider other)
        {

            if (other.CompareTag("HMD"))
            {
                StartCoroutine(Fade(false));
                //RehaStimInterface.instance.enableEMS = true;
                AmbientSoundSwitcher.switchSound("Room3");
            }
        }

        public void ToggleFade()
        {
            toblack = !toblack;
            StartCoroutine(Fade(toblack));
        }

        // Update is called once per frame
        void Update()
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;

            color = panel.color;
            color.a = alpha;
            panel.color = color;
        }

        IEnumerator Fade(bool toBlack)
        {
            if (toBlack)
            {
                while (alpha < 1)
                {
                    alpha += Time.deltaTime / fadeTime;
                    yield return null;
                }
            }
            else
            {

                while (alpha > 0)
                {
                    alpha -= Time.deltaTime / fadeTime;
                    yield return null;
                }
            }
            yield return null;
        }
    }
}