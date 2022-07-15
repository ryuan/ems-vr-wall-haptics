using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MuscleDeck
{
    public class RoomTeleporter : MonoBehaviour
    {
        public int teleportTargetIndex = -1;
        public bool canTeleport = false;

        protected virtual bool OnTriggerEnter(Collider other)
        {
            if (checkCollider(other) && canTeleport)
            {
                if (teleportTargetIndex >=0)
                {
                    print(teleportTargetIndex);
                    print(canTeleport);
                    canTeleport = false;
                    SwitchRooms.instance.SwitchRoom(teleportTargetIndex);
                }

                return true;
            }

            return false;
        }

        protected virtual bool checkCollider(Collider other)
        {
            return other.CompareTag("HMD");
        }
    }
}
