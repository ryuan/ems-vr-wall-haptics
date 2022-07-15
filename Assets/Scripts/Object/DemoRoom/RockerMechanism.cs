using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MuscleDeck
{
    public class RockerMechanism : MonoBehaviour
    {
        /* 0 = fully pushed right, 1 fully pushed left*/
        [Range(0, 1)]
        public float position;

        public RockerButtonScript rockerL;
        public RockerButtonScript rockerR;

        public float minDifference;
        public float lastDirection = 0f;
        public float speed;

        public PumpReceiver receiver;
        [Range(.1f, 1)]
        public float minPumpDepth;

        protected RockerCollider rockerColliderR;
        protected RockerCollider rockerColliderL;
        private float max = 1f;

        public virtual void Start()
        {
            rockerColliderL = rockerL.GetComponentInChildren<RockerCollider>();
            rockerColliderR = rockerR.GetComponentInChildren<RockerCollider>();
        }

        public virtual void Update()
        {
            float direction = (rockerColliderR.penetration - rockerColliderL.penetration);
            
            if (Mathf.Abs(direction) > minDifference)
                position += direction * speed * Time.deltaTime;

            position = Mathf.Clamp(position, 0f, 1f);

            rockerL.z = (1 - position) * max;
            rockerR.z = position * max;

            // Check pumping 
            if (
                lastDirection != direction 
                &&  (( position < 1 - minPumpDepth && lastDirection > 0)
                    || (position > minPumpDepth && lastDirection < 0))
                )
            {
                receiver.onPress("");
                lastDirection = direction;
            }
            else if ( lastDirection == 0 )
            {
                lastDirection = direction;
            }
            
        }
    }
}
