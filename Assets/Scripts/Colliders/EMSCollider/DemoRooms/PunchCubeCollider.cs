using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MuscleDeck
{
    public class PunchCubeCollider: RepulsionCollider
    {
        [Tooltip("How much to amplify the hand velocity")]
        public float forceFactor = 10;

        public float forceThreshhold = 0.5f;
        protected override void ApplyRepulsion()
        {
            // get the current velocity of the hand
            Vector3 velocity = new Vector3();

            VelocityTracker script = trackingObject.gameObject.GetComponent<VelocityTracker>();

            if (script)
            {
                velocity = script.velocity;
            }

            Side side = trackingObject.CompareTag("HandL") ?
                        Side.Left :
                        Side.Right;

            Client.Instance.SendMessage(getMessageType(),
                new PunchCubePayload(velocity, side));

            //apply force to rigidbody
            if ((script.velocity * forceFactor).magnitude > forceThreshhold)
                transform.parent.GetComponent<Rigidbody>().AddForce(script.velocity * forceFactor, ForceMode.Impulse);
        }

        protected override MessageType getMessageType()
        {
            return MessageType.PunchCube;
        }
    }
}
