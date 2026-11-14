using System;
using System.Collections.Generic;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;

namespace Duckov.Consoles;

public class DConsole : MonoBehaviour
{
	[SerializeField]
	private FadeGroup uiFadeGroup;

	[SerializeField]
	private TMP_InputField inputField;

	public List<DCommand> defaultCommands = new List<DCommand>();

	private int frameCounter;

	public static Dictionary<string, IDCommand> commandDic = new Dictionary<string, IDCommand>();

	public static DConsole Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Multiple DConsole instances found in scene.");
		}
		Instance = this;
		foreach (DCommand defaultCommand in defaultCommands)
		{
			RegisterCommand(defaultCommand);
		}
		TMP_InputField tMP_InputField = inputField;
		tMP_InputField.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(tMP_InputField.onValidateInput, new TMP_InputField.OnValidateInput(OnValidateInput));
	}

	private char OnValidateInput(string text, int charIndex, char addedChar)
	{
		if (addedChar == '`')
		{
			HideConsole();
		}
		return addedChar;
	}

	private void Update()
	{
		if (!CheatMode.Active)
		{
			return;
		}
		frameCounter++;
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			if (uiFadeGroup.IsShown)
			{
				HideConsole();
			}
			else if (frameCounter > 1)
			{
				ShowConsole();
			}
		}
		if (uiFadeGroup.IsShown && Input.GetKeyDown(KeyCode.Return))
		{
			string text = inputField.text;
			inputField.text = string.Empty;
			Process(text);
			uiFadeGroup.Hide();
		}
	}

	private void ShowConsole()
	{
		uiFadeGroup.Show();
		inputField.text = string.Empty;
		inputField.ActivateInputField();
	}

	private void HideConsole()
	{
		inputField.text = string.Empty;
		uiFadeGroup.Hide();
		frameCounter = 0;
	}

	public static void RegisterCommand(IDCommand command)
	{
		if (string.IsNullOrWhiteSpace(command.CommandWord))
		{
			Debug.LogError("Empty command word.");
			return;
		}
		if (commandDic.ContainsKey(command.CommandWord))
		{
			Debug.LogError("Duplicate command: " + command.CommandWord);
		}
		commandDic[command.CommandWord] = command;
	}

	public void Process(string command)
	{
		if (string.IsNullOrWhiteSpace(command))
		{
			return;
		}
		command = command.Trim();
		string[] array = command.Split(" ");
		string text = array[0];
		if (!TryGetCommand(text, out var dCommand))
		{
			EchoWarning("Invalid Command: " + text);
			return;
		}
		string[] array2 = new string[array.Length - 1];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = array[i + 1];
		}
		string result = dCommand.Execute(this, array2);
		Echo(result);
	}

	public void Echo(string result)
	{
		Debug.Log(result);
	}

	public void EchoWarning(string message)
	{
		Debug.LogWarning(message);
	}

	private bool TryGetCommand(string commandWord, out IDCommand dCommand)
	{
		commandWord = commandWord.Trim();
		if (!commandDic.TryGetValue(commandWord, out var value))
		{
			dCommand = value;
			return false;
		}
		dCommand = value;
		return true;
	}
}
