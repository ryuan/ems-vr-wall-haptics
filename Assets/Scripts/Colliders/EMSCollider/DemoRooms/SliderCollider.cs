using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MuscleDeck
{
    public class SliderCollider : CubeCollider
    {
        public float minSpeed = .0f;
        public float maxSpeed = 1f;

        public float minPushDepth = 0.1f;
        public float maxDisplacement = .5f;

        protected override void Update()
        {
            base.Update();

            if(trackingObject)
            {
                Vector3 pos = transform.InverseTransformPoint(trackingObject.transform.position);
                int direction;

                float speed = 0;

                if (Mathf.Abs(pos.z) > minPushDepth)
                    speed = Mathf.Lerp(maxSpeed, minSpeed, Mathf.Abs(pos.z) * 2f);

                if (pos.z > 0)
                { // From object*s left
                    direction = -1;
                }
                else
                { // From object*s right
                    direction = 1;
                }

                Vector3 newPos = transform.localPosition;
                newPos.z = Mathf.Clamp(
                    newPos.z + direction * Time.deltaTime * speed, 
                    -maxDisplacement, 
                    maxDisplacement);
                transform.localPosition = newPos;
            }
        }

        protected override MessageType GetMessageType()
        {
            return MessageType.Slider;
        }
    }
}
