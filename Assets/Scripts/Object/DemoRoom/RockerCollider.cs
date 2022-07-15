using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MuscleDeck
{
    public class RockerCollider : StickyCollider {
        public float penetration;
    
        protected override void Update()
        {
            base.Update();

            if (trackingObject)
            {
                Vector3 pos = transform.InverseTransformPoint(trackingObject.transform.position);

                penetration = pos.z * 2;

                //print(penetration);

                Client.Instance.SendMessage(
                    MessageType.RockerButton,
                    new ContinuousPayload(
                        penetration, 
                        1
                        ,getSide())
                );
            }
        }

        protected override void ResetCollider()
        {
            base.ResetCollider();

            penetration = 0;
        }


    }
}
