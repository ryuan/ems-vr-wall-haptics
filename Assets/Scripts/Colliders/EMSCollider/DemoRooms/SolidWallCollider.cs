using System;
using System.Collections.Generic;
using UnityEngine;

namespace MuscleDeck
{
    public class SolidWallCollider : ContinuousCollider
    {
        public bool active = false;

        public DateTime deactivateTime = DateTime.Now;

        protected override void Update()
        {
            base.Update();

            if (!trackingObject && DateTime.Now > deactivateTime)
            {
                active = false;
            }
        } 

        protected override MessageType GetMessageType()
        {
            return active ?  
                MessageType.SolidWall : 
                MessageType.Unknown;
        }

        protected override bool OnTriggerEnter(Collider other)
        {
            if (!active)
                return false;

            return base.OnTriggerEnter(other);
        }

        protected bool OnTriggerStay(Collider other)
        {
            if (!active || trackingObject)
                return false;

            return base.OnTriggerEnter(other);
        }

        protected override bool OnTriggerExit(Collider other)
        {
            if (!base.OnTriggerExit(other))
                return false;

            active = false;

            return true;
        }
    }
}
