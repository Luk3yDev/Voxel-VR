using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] TMP_Text playerName;

    NetworkVariable<NetworkString> networkPlayerName = new NetworkVariable<NetworkString>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            networkPlayerName.Value = GameObject.Find("Network Manager").GetComponent<NetworkConnect>().nameInput.text;
        }

        playerName.text = networkPlayerName.Value.ToString();       
    }

    void NetworkPlayerName_OnValueChanged(NetworkString previousValue, NetworkString newValue)
    {
        playerName.text = newValue;
    }

    private void Awake()
    {
        networkPlayerName.OnValueChanged += NetworkPlayerName_OnValueChanged;
    }
}

public struct NetworkString : INetworkSerializeByMemcpy
{
    private ForceNetworkSerializeByMemcpy<FixedString32Bytes> _info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
    where T : IReaderWriter
    {
        serializer.SerializeValue(ref _info);
    }

    public override string ToString()
    {
        return _info.Value.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();
    public static implicit operator NetworkString(string s) => new NetworkString() { _info = new FixedString32Bytes(s) };
}