using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class CommandList : MonoBehaviour
{
    Dictionary<string, System.Action<string>> commands = new Dictionary<string, System.Action<string>>();

    void Start()
    {
        RegisterCommands();
    }

    void RegisterCommands()
    {
        commands.Add("help", (input) => DevConsole.Instance.debugText.text = ("Available Commands: " + string.Join(", ", commands.Keys)));
        commands.Add("quit", (input) => Application.Quit());

        //AI Card Draw Commands
        commands.Add("manualdraw", (input) => 
        {
            AICardDrawSystem.Instance.ToggleManualDrawMode();
        });

        commands.Add("listcards", (input) => 
        {
            AICardDrawSystem.Instance.ListAvailableCards();
        });

        commands.Add("aidraw", (input) => 
        {
            string[] args = input.Split(' ');
            if (args.Length < 2)
            {
                DevConsole.Instance.debugText.text = ("Usage: aidraw [keyword1] [keyword2] ...");
                return;
            }

            List<string> cardNames = new List<string>(args.Skip(1));
            AICardDrawSystem.Instance.SetNextDrawCards(cardNames.ToArray());
        });
    }

    public bool ExecuteCommand(string input)
    {
        string[] args = input.Split(' ');
        string commandName = args[0];

        if (commands.ContainsKey(commandName))
        {
            commands[commandName].Invoke(input);
            return true;
        }

        DevConsole.Instance.debugText.text = ($"Unknown Command: {commandName}");
        return false;
    }

    public string GetAutoCompleteSuggestion(string input)
    {
        //Display Available Cards When aidraw Command Typed
        if (input.StartsWith("aidraw ", StringComparison.OrdinalIgnoreCase))
        {
            //Get The Part After "aidraw " Text
            string cardInput = input.Substring(7).ToLower();

            List<string> availableCardNames = CardDeck.Instance.deck
            .Select(card => card.name)
            .Distinct()
            .ToList();

            //If The Input Is Empty Show All Available Cards
            if (string.IsNullOrWhiteSpace(cardInput))
            {
                string allCardsDisplay = string.Join(", ", availableCardNames);
                return "Available cards: " + allCardsDisplay + ", ";
            }
            else
            {
                //Get Card Names That Start With The Input
                var matchingCardNames = availableCardNames
                    .Where(name => name.ToLower().StartsWith(cardInput))
                    .ToList();

                //Return First Match As The Suggestion
                if (matchingCardNames.Count > 0)
                {
                    return "aidraw " + matchingCardNames[0];
                }
            }
        }
        else
        {
            foreach (var cmd in commands.Keys)
            {
                if (cmd.StartsWith(input))
                    return cmd;
            }
        }

        return "";
    }
}
