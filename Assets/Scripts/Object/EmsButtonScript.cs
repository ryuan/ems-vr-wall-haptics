using UnityEngine;
using System.Collections;

namespace MuscleDeck
{
    public class EmsButtonScript : ButtonScript
{

    protected override void Update()
    {
        base.Update();

        if (trackingObject)
        {
            float z = transform.InverseTransformPoint(trackingObject.transform.position).z;

            MessageType type = GetUpdateMessageType();
            if (type != MessageType.Unknown)
            {
                Client.Instance.SendMessage(
                    type,   
                    new ButtonPayload(z, minPenetration, maxPenetration, getSide())
                );
            }
        }
    }

    protected virtual MessageType GetUpdateMessageType()
    {
        return MessageType.ButtonUpdate;
    }

    protected override bool OnTriggerEnter(Collider other)
    {
        if (!base.OnTriggerEnter(other))
            return false;

        MessageType type = GetContactMessageType();
        if (type != MessageType.Unknown)
        {
            Client.Instance.SendMessage(type);
        }

        return true;
    }

    protected virtual MessageType GetContactMessageType()
    {
        return MessageType.Unknown;
    }

    protected override void ResetCollider()
    {
        base.ResetCollider();

        Client.Instance.SendMessage(
            MessageType.Stop
        );

        }
    }
}