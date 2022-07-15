using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MuscleDeck
{
    public class SolidObjectCollider : StickyCollider
    {
        protected override void Update()
        {
            base.Update();

            if (trackingObject)
            {
                Vector3 pos = transform.InverseTransformPoint(trackingObject.transform.position);

                Client.Instance.SendMessage(GetMessageType(),
                    new SolidObjectPayload(
                        new Vector3[]
                        {
                            Camera.main.transform.right,
                            Camera.main.transform.up,
                            Camera.main.transform.forward
                        },
                        pos,
                        trackingObject.transform.forward,
                        getSide(),
                        gameObject.name)
                );
            }
        }


        protected override void ResetCollider()
        {
            base.ResetCollider();

            Client.Instance.SendMessage(MessageType.Stop);
        }

        protected virtual MessageType GetMessageType()
        {
            return MessageType.SolidObject;
        }
    }
}
