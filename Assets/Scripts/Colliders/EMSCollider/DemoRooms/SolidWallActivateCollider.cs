using System;
using System.Collections.Generic;
using UnityEngine;

namespace MuscleDeck
{
    public class SolidWallActivateCollider : StickyCollider
    {
        public SolidWallCollider targetCollider;

        protected override bool OnTriggerEnter(Collider other)
        {
            if (!base.OnTriggerEnter(other))
                return false;

            targetCollider.active = true;
            targetCollider.deactivateTime = DateTime.Now.AddMilliseconds(1000);

            return true;
        }
    }
}
