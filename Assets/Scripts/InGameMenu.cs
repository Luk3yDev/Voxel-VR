using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] SnapTurnProviderBase SnapTurnProvider;

    public void ToggleTurning(Image image)
    {
        SnapTurnProvider.enabled = !SnapTurnProvider.enabled;
        if (SnapTurnProvider.enabled ) image.color = Color.white;
        else image.color = Color.black;
    }
}
