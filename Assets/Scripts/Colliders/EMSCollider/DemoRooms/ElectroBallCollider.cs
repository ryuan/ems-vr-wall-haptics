using UnityEngine;
using System;

namespace MuscleDeck
{
    [RequireComponent(typeof(Rigidbody))]
    public class ElectroBallCollider : ElectroWallCollider
    {
        protected bool targeting;
        protected bool dying;
        protected bool stimulate = true;
        protected DateTime dieTime = DateTime.Now;
        private GameObject _target;
        [Header("Other Settings")]
        public float speed = 1f;

        protected override MessageType getMessageType()
        {
            return MessageType.ElectroProjectile;
        }

        protected override int getCooldown()
        {
            return 5000;
        }

        protected override void Update()
        {
            base.Update();

            if (targeting)
            {
                Rigidbody rigidbody = GetComponent<Rigidbody>();
                Vector3 pos = Vector3.MoveTowards(transform.position, _target.transform.position, speed * Time.fixedDeltaTime);
                rigidbody.MovePosition(pos);
                //transform.position = pos;
                //print(pos);
                if (Vector3.Distance(transform.position, _target.transform.position) < 0.01f)
                {
                    stimulate = false;
                    onHit();
                }
            }
            if (dying && DateTime.Now > dieTime)
            {
                Destroy(gameObject);
            }
        }

        protected override bool checkCollider(Collider other)
        {
            return base.checkCollider(other) && stimulate;
        }

        protected override bool OnTriggerEnter(Collider other)
        {
            if( !base.OnTriggerEnter(other))
                return false;

            onHit();

            return true;
        }

        private void onHit()
        {
            Vector3 hitVector = _target.transform.forward * 10;

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            targeting = false;
            rigidbody.isKinematic = false;
            rigidbody.AddForce(hitVector);
            rigidbody.useGravity = true;

            dying = true;
            dieTime = DateTime.Now.AddMilliseconds(1500);
        }

        public void init()
        {
            _target = Camera.main.gameObject;
            targeting = true;
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddTorque(new Vector3(0, 1, 1), ForceMode.VelocityChange);
        }
    }
}