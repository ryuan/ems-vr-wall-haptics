using UnityEngine;
using System;

namespace MuscleDeck
{
    public class WaterCollider : MonoBehaviour
    {
        protected Vector3 curVelocity;
        protected GameObject trackedObject;
        protected Vector3 lastPosition;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (trackedObject)
            {
                Vector3 delta = trackedObject.transform.position - lastPosition;
                delta = delta / Time.deltaTime;

                float interpolationFactor = 0.8f;
                curVelocity = curVelocity * interpolationFactor + delta * (1 - interpolationFactor);

                float direction = Vector3.Dot(curVelocity, trackedObject.transform.up);

                Side side = trackedObject.CompareTag("HandL") ? Side.Left : Side.Right;

                Message msg = new Message(MessageType.Water, new WaterPayload(side, direction));

                lastPosition = trackedObject.transform.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!checkObject(other.gameObject))
                return;

            trackedObject = other.gameObject;
            curVelocity = new Vector3();
            lastPosition = other.transform.position;
        }

        private void OnTriggerLeave(Collider other)
        {
            if (other.gameObject == trackedObject)
            {
                Reset();
            }
        }

        private void Reset()
        {
            ChannelList.Stop();
            trackedObject = null;
        }

        private bool checkObject(GameObject other)
        {
            return other.CompareTag("HandR") 
                || other.CompareTag("HandL");
        }
    }
}