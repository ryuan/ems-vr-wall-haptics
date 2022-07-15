using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MuscleDeck
{
    public class RockerButtonScript : MonoBehaviour
    {
        protected Transform child;
        public float speed = 0.5f;
        protected Vector3 childStartPos;
        [Range(0,1)]
        public float z;

        protected /*override*/ void Start()
        {
            child = transform.GetChild(0).GetChild(0).GetChild(0);
            childStartPos = child.localPosition;
        }

        protected /*override*/ void Update()
        {
            //base.Update();
            child.localPosition = new Vector3(
                child.localPosition.x,
                child.localPosition.y,
                z 
            );
        }

    }
}
