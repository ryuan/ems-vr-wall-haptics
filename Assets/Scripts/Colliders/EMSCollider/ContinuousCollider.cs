using UnityEngine;
using System;

namespace MuscleDeck
{
    public class ContinuousCollider : StickyCollider
    {
        protected Vector3 startPos;

        protected override void Update()
        {
            base.Update();

            if (trackingObject)
            {
                Vector3 position = transform.InverseTransformPoint(
                    trackingObject.transform.position);

                if (GetMessageType() != MessageType.Unknown)
                {
                    Client.Instance.SendMessage(
                        GetMessageType(),
                        new ContinuousPayload(
                            position.magnitude,
                            1.41f,
                            trackingObject.CompareTag("HandL") ? Side.Left : Side.Right
                        )
                    );
                }
            }
        }

        protected virtual MessageType GetMessageType()
        {
            return MessageType.Unknown;
        }

        protected override bool OnTriggerEnter(Collider other)
        {
            if (!base.OnTriggerEnter(other))
                return false;

            startPos = transform.InverseTransformPoint(
                    trackingObject.transform.position);

            return true;
        }
    }
}