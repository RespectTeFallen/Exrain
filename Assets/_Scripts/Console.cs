using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Console : MonoBehaviour
{
    #region Singleton
    public static Console instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }
    #endregion

    public GameObject ConsoleWindow;
    public TMP_InputField consoleText;
    public TextMeshProUGUI consoleOutput;

    public List<string> commandList;

    private string prefix;
    private string suffix;
    private string value;
    private int index;

    public void SendInput(string command)
    {
        if (command.Contains(" "))
        {
            index = command.IndexOf(' ');
            prefix = command.Substring(0, index);
            suffix = command.Substring(index + 1);
            Debug.Log(prefix + "prefix");
            if (suffix.Contains(" "))
            {
                index = suffix.IndexOf(' ');
                value = suffix.Substring(index + 1);
                suffix = suffix.Substring(0, index);
                Debug.Log(suffix + "suffix");
                Debug.Log(value + "value");
            }
        }
        else
        {
            index = 0;
            prefix = null;
            value = null;
        }

        switch (command)
        {
            case "help":
                for (int i = 0; i < commandList.Count; i++)
                {
                    consoleOutput.text = consoleOutput.text + "\n" + commandList[i];
                }
                break;
            case "reset":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            default:
                Prefix();
                break;
        }

        void Prefix()
        {
            switch (prefix)
            {
                case "give":
                    consoleOutput.text = consoleOutput.text + "\ngive " + suffix + value;
                    Inventory.instance.AddItem("inventory", new Item(suffix, 5, "item " + suffix, int.Parse(value), Item.ItemType.Item));
                    return;
                case "fps":
                    if (int.Parse(suffix) == 0)
                    {
                        PlayerController.instance.fps.enabled = false;
                    }
                    else if (int.Parse(suffix) == 1)
                    {
                        PlayerController.instance.fps.enabled = true;
                    }
                    else
                    {
                        consoleOutput.text = consoleOutput.text + "\nIncorrect input: " + command;
                        return;
                    }
                    consoleOutput.text = consoleOutput.text + "\nfps " + suffix;
                    return;
                default:
                    consoleOutput.text = consoleOutput.text + "\nIncorrect input: " + command;
                    return;
            }
        }
        consoleText.ActivateInputField();
    }
}
