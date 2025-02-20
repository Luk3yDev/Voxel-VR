using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.UI;

public class VRKeyboard : MonoBehaviour
{
    [SerializeField] RectTransform rowPrefab;
    [SerializeField] RectTransform keyPrefab;

    // _ means ignore,  * means backspace
    char[,] keyboardStructure =
    {
        {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0'},
        {'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p'},
        {'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', '_'},
        {'z', 'x', 'c', 'v', 'b', 'n', 'm', '_', '_', '*'}
    };

    public event OnCharEvent OnKeyPressed;
    public delegate void OnCharEvent(char key);

    private void Start()
    {
        GenerateKeyboard();
    }

    void GenerateKeyboard()
    {
        for (int rowID = 0; rowID < keyboardStructure.GetLength(0); rowID++)
        {
            RectTransform rowParent = Instantiate(rowPrefab, transform.GetChild(0));
            for (int colID = 0; colID < keyboardStructure.GetLength(1); colID++)
            {
                char c = keyboardStructure[rowID, colID];
                if (keyboardStructure[rowID, colID] != '_')
                {
                    RectTransform key = Instantiate(keyPrefab, rowParent);
                    if (c != '*')
                        key.GetChild(0).GetComponent<TMP_Text>().text = keyboardStructure[rowID, colID].ToString().ToUpper();
                    else
                        key.GetChild(0).GetComponent<TMP_Text>().text = "BACK\nSPACE";
                    key.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
                    {
                        KeyPressed(c);
                    }));
                }
            }
        }
    }

    void test(char key)
    {
        //Debug.Log(key);
    }

    // I don't fucking know
    bool nahG = false;
    void KeyPressed(char key)
    {
        if (!nahG)
        {
            OnKeyPressed?.Invoke(key);
            nahG = true;
        }
        else nahG = false;
    }
}
