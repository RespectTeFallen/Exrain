using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Console : MonoBehaviour
{

    public static Console instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public GameObject ConsoleWindow;
    public TMP_InputField consoleText;
    public TextMeshProUGUI consoleOutput;

    private string prefix;

    public void SendInput(string command)
    {
        if (command.Length >= 5)
        {
            prefix = command.Substring(0, 5);
        }

        switch (command)
        {
        case "test":
                consoleOutput.text = consoleOutput.text + "\n" + command;
                break;
        default:
                Prefix();
                break;
        }

        void Prefix()
        {
            Debug.Log(prefix);
            switch (prefix)
            {
                case "give ":
                    consoleOutput.text = consoleOutput.text + "\n" + command;
                    return;
                default:
                    consoleOutput.text = consoleOutput.text + "\nIncorrect input: " + command;
                    return;
            }
        }
    }

    void test()
    {
        Debug.Log("test");
    }
}
