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

    public void ToggleTurning(Image image)
    {
        SnapTurnProvider.enabled = !SnapTurnProvider.enabled;
        if (SnapTurnProvider.enabled ) image.color = Color.white;
        else image.color = Color.black;
    }

    public void ToggleCamera(Image ui)
    {
        selfieCamera.SetActive(!selfieCamera.activeSelf);
        if (selfieCamera.activeSelf) ui.color = Color.white;
        else ui.color = Color.black;
    }

    public void TakeScreenshot()
    {
        string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string fileName = "Voxel VR Screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string fullPath = Path.Combine(picturesPath, fileName);

        ScreenCapture.CaptureScreenshot(fullPath);
    }
}
