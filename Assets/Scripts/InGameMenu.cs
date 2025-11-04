using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System;
using System.IO;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameObject UI;
    [SerializeField] GameObject selfieCamera;
    [SerializeField] SnapTurnProviderBase SnapTurnProvider;
    public InputActionProperty buttonAction;

    private void Update()
    {
        if (buttonAction.action.triggered)
        {
            UI.SetActive(!UI.gameObject.activeSelf);
        }
    }

    private float lastToggleTurnTime;
    public float toggleTurnCooldown = 0.2f;
    public void ToggleTurn(Image ui)
    {
        if (Time.time - lastToggleTurnTime < toggleTurnCooldown) return;
        lastToggleTurnTime = Time.time;

        SnapTurnProvider.enabled = !SnapTurnProvider.enabled;
        ui.color = SnapTurnProvider.enabled ? Color.white : Color.black;
    }

    private float lastToggleTime;
    public float toggleCooldown = 0.2f;
    public void ToggleCamera(Image ui)
    {
        if (Time.time - lastToggleTime < toggleCooldown) return;
        lastToggleTime = Time.time;

        selfieCamera.SetActive(!selfieCamera.activeSelf);
        ui.color = selfieCamera.activeSelf ? Color.white : Color.black;
    }


    public void TakeScreenshot()
    {
        UI.SetActive(false);

        string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string fileName = "Voxel VR Screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string fullPath = Path.Combine(picturesPath, fileName);

        ScreenCapture.CaptureScreenshot(fullPath);
    }
}
