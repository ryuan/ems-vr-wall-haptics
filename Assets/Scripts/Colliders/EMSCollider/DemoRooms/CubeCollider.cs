using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MuscleDeck
{
    public class CubeCollider : SolidObjectCollider
    {
        protected override MessageType GetMessageType()
        {
            return MessageType.Cube;
        }

        public void ResetCubeStim()
        {
            ResetCollider();
        } 
    }
}
