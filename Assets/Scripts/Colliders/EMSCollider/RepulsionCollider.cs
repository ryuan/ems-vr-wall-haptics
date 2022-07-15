using UnityEngine;
using System;

namespace MuscleDeck
{
	public class RepulsionCollider : StickyCollider {

        protected DateTime activeTime = DateTime.Now;


        protected override void Update()
        {
            base.Update();

            if (trackingObject)
            {
                if (activeTime < DateTime.Now)
                {
                    ApplyRepulsion();

                    activeTime = DateTime.Now.AddMilliseconds(getCooldown());
                }
            }
        }

        protected virtual void ApplyRepulsion()
        {
            Side side = trackingObject.CompareTag("HandR") ?
                        Side.Right :
                        Side.Left;

            Client.Instance.SendMessage(getMessageType(),
                new RepulsionPayload(side));
        }

        protected virtual MessageType getMessageType()
        {
            return MessageType.Unknown;
        }

        protected virtual int getCooldown()
        {
            return 1000;
        }

        protected override bool checkCollider(Collider other)
        {
            return other.gameObject.CompareTag("HandR") ||
                other.gameObject.CompareTag("HandL")
                ;
        }
    }
}