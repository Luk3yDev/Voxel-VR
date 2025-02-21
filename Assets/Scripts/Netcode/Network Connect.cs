using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnect : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;

    public TMP_InputField joinCodeInput;
    public TMP_InputField nameInput;
    public TMP_Text joinCodeText;

    public GameObject startGameButton;

    VRKeyboard vrKeyboard;

    private void Awake()
    {
        vrKeyboard = FindObjectOfType<VRKeyboard>();    
    }

    void OnKeyboardKeyPress(char key)
    {
        if (key != '*')
            joinCodeInput.text = joinCodeInput.text + key;
        else
        {
            if (joinCodeInput.text.Length > 0)
                joinCodeInput.text = joinCodeInput.text.Remove(joinCodeInput.text.Length-1);
        }
    }

    async void Start()
    {
        vrKeyboard.OnKeyPressed += OnKeyboardKeyPress;

        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.onClick.AddListener(CreateRelay);
        joinButton.onClick.AddListener(() => JoinRelay(joinCodeInput.text));
    }

    bool nahBro2 = false;
    async void CreateRelay()
    {
        if (nahBro2) return;
        else
        {
            nahBro2 = false;
        }
        nahBro2 = true;

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        joinCodeText.text = $"Code: {joinCode}";
        Debug.Log($"Hosted at: {joinCode}");

        var relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();

        GameObject.Find("World").GetComponent<NoiseGenerator>().GenerateSeed(joinCode);

        startGameButton.SetActive(true);
    }

    bool nahBro = false;
    async void JoinRelay(string joinCode)
    {
        if (nahBro) return;
        else
        {
            nahBro = false;
        }
        nahBro = true;

        joinCode = joinCode.Substring(0, 6);
        Debug.Log($"Tried to join: {joinCode.ToUpper()}");

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode.ToUpper());
        var relayServerData = new RelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();

        GameObject.Find("World").GetComponent<NoiseGenerator>().GenerateSeed(joinCode.ToUpper());
    }

    // TextEditor te = new TextEditor(); te.text = joinCode; te.SelectAll(); te.Copy(); CLIPBOARD CODE FOR LATER
}
