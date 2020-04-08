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

    private List<string> lastCommand = new List<string>();
    private int commandCount;

    private string prefix;
    private string suffix;
    private string value;
    private int index;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && lastCommand.Count > 0 && commandCount > 0)
        {
            commandCount -= 1;
            consoleText.text = lastCommand[commandCount];
            consoleText.caretPosition = consoleText.text.Length;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && lastCommand.Count > 1 && commandCount < lastCommand.Count - 1)
        {
            commandCount += 1;
            consoleText.text = lastCommand[commandCount];
            consoleText.caretPosition = consoleText.text.Length;
        }
    }

    public void SendInput(string command)
    {
        if (command.Contains(" "))
        {
            index = command.IndexOf(' ');
            prefix = command.Substring(0, index);
            suffix = command.Substring(index + 1);
            if (suffix.Contains(" "))
            {
                index = suffix.IndexOf(' ');
                value = suffix.Substring(index + 1);
                suffix = suffix.Substring(0, index);
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
                lastCommand.Add(command);
                commandCount = lastCommand.Count;
                break;
            case "clear":
                for (int i = 0; i < Inventory.instance.inventory.Count; i++)
                {
                    if (Inventory.instance.inventory[i].itemID != 0)
                    {
                        Inventory.instance.inventory[i] = new Item("empty", 0, "", 0, false, "", Item.ItemType.Null);
                    }
                }
                return;
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
                    consoleOutput.text = consoleOutput.text + "\ngive " + suffix + " " + value;
                    Item itemPlace = ItemDatabase.instance.itemList[suffix];
                    Inventory.instance.AddItem("inventory", new Item(itemPlace.itemName, itemPlace.itemID, itemPlace.itemDesc, int.Parse(value), itemPlace.itemStackable, itemPlace.itemData, itemPlace.itemType));
                    lastCommand.Add(command);
                    commandCount = lastCommand.Count;
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
                        lastCommand.Add(command);
                        commandCount = lastCommand.Count;
                        return;
                    }
                    consoleOutput.text = consoleOutput.text + "\nfps " + suffix;
                    lastCommand.Add(command);
                    commandCount = lastCommand.Count;
                    return;
                default:
                    consoleOutput.text = consoleOutput.text + "\nIncorrect input: " + command;
                    lastCommand.Add(command);
                    commandCount = lastCommand.Count;
                    return;
            }
        }
        consoleText.ActivateInputField();
    }
}
