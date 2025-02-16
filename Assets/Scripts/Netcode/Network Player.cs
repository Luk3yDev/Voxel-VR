using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        root.position = VRRigRef.Singleton.root.position;
        root.rotation = VRRigRef.Singleton.root.rotation;

        head.position = VRRigRef.Singleton.head.position;
        head.rotation = VRRigRef.Singleton.head.rotation;

        leftHand.position = VRRigRef.Singleton.leftHand.position;
        leftHand.rotation = VRRigRef.Singleton.leftHand.rotation;

        rightHand.position = VRRigRef.Singleton.rightHand.position;
        rightHand.rotation = VRRigRef.Singleton.rightHand.rotation;
    }
}
