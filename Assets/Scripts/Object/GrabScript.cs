using UnityEngine;
using System.Collections.Generic;
using System;

/*
 *Follow the hands after being grabbed
 */

namespace MuscleDeck
{
    public class GrabScript : MonoBehaviour
    {
        private GameObject handL = null;
        private GameObject handR = null;

        public Vector3 velocity;
        public float factor = 100f;

        private Vector3 lastPosition;
        private Vector3 startPosition;
        private Quaternion startRotation;

        public AudioSource grabSound;

        public KeyCode resetButton = KeyCode.KeypadEnter;
        private new Rigidbody rigidbody;

        protected Transform parent;
        private void OnTriggerEnter(Collider other)
        {
            if (!
                (other.gameObject.CompareTag("CubeGrabR")
                || other.gameObject.CompareTag("CubeGrabL")
                )
            )
                return;

            if (other.CompareTag("CubeGrabL"))
            {
                handL = other.gameObject;
            }
            else
            {
                handR = other.gameObject;
            }

            if (handL != null && handR != null)
            {
                grabSound.Play();
                Vector3 direction = (handR.transform.position - handL.transform.position);
                direction.Scale(new Vector3(.5f, .5f, .5f));
                offset = parent.position - (handL.transform.position + direction);

                //rotationOffset = Quaternion.LookRotation(direction, handL.transform.up);

                //parent.gamerigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }
        }

        private void Start()
        {
            parent = transform.parent;
            lastPosition = parent.position;
            startPosition = parent.localPosition;
            startRotation = parent.localRotation;
            rigidbody = parent.GetComponent<Rigidbody>();
            velocities = new LinkedList<Vector3>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(resetButton))
            {
                parent.localPosition = startPosition;
                parent.localRotation = startRotation;
                rigidbody.velocity = new Vector3();
                rigidbody.angularVelocity = new Vector3();
            }

            if (handL != null && handR != null)
            {
                Vector3 direction = (handR.transform.position - handL.transform.position);
                direction.Scale(new Vector3(.5f, .5f, .5f));
                Vector3 pos = handL.transform.position + direction;
                parent.position = pos + offset;

                Vector3 up = Vector3.Angle(direction, handL.transform.forward) > 
                    Vector3.Angle(direction, handL.transform.up) ?
                    handL.transform.forward :
                     handL.transform.up
                    ;

                Quaternion rotation = Quaternion.LookRotation(direction, up);
                //Quaternion rotation = Quaternion.LookRotation(
                //    Camera.main.transform.forward, 
                //    handL.transform.up);
                // update rotation
                parent.rotation = rotation /** rotationOffset*/;
                
                if (colliderEms.enabled)
                {
                    colliderEms.gameObject.GetComponent<CubeCollider>().ResetCubeStim();
                    colliderEms.enabled = false;
                }
                Client.Instance.SendMessage(MessageType.CubeGrab);
            }

            UpdateVelocity();

            if (changeCollider && colliderTime< DateTime.Now )
            {
                handModelL.GetComponent<BoxCollider>().enabled = true;
                handModelR.GetComponent<BoxCollider>().enabled = true;
                colliderEms.enabled = true;

                changeCollider = false;
            }
        }


        // Lot of Hacks starting here
        //public Vector3 testVector;
        //public float test;
        protected DateTime colliderTime;
        protected bool changeCollider;
        public GameObject handModelL;
        public GameObject handModelR;
        public BoxCollider colliderEms;
        protected LinkedList<Vector3> velocities;
        public int numVelocities = 5;

        private void UpdateVelocity()
        {
            if (velocities.Count >= numVelocities)
            {
                velocities.RemoveFirst();
            }

            Vector3 vel = (parent.position - lastPosition)
                * Time.deltaTime
                * factor;

            velocities.AddLast(vel);

            velocity = new Vector3();

            foreach (var velo in velocities)
            {
                velocity += velo;
            }

            velocity /= velocities.Count;
            //velocity = vel;

            lastPosition = parent.position;
        }


        //public int refreshRate = 100;
        private float timer;
        private Vector3 offset;
        private Quaternion rotationOffset;

        private void FixedUpdate()
        {
            //    if (timer <= 0)
            //    {
                //velocity = parent.position - lastPosition;
                //velocity *= Time.deltaTime;
                //velocity *= factor;

                //lastPosition = parent.position;
            //    timer = refreshRate / 1000.0f;
            //}
            //else
            //{
            //    timer -= Time.fixedDeltaTime;
            //}
        }

        private void OnTriggerExit(Collider other)
        {
            if (!
                (other.gameObject.CompareTag("CubeGrabR")
                || other.gameObject.CompareTag("CubeGrabL")
                )
            )
                return;

            if (other.CompareTag("CubeGrabL"))
            {
                handL = null;
            }
            else
            {   
                handR = null;
            }

            if (handL != null || handR != null)
            {
                handModelL.GetComponent<BoxCollider>().enabled = false;
                handModelR.GetComponent<BoxCollider>().enabled = false;
                colliderTime = DateTime.Now.AddMilliseconds(500);
                changeCollider = true;

                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;

                if (handR != null)
                    rigidbody.AddForce(
                        new Vector3(
                            velocity.x,
                            velocity.y * 2,
                            velocity.z
                        )
                        , ForceMode.VelocityChange);

                Client.Instance.SendMessage(MessageType.Stop);
            }
        }
    }
}