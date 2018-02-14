using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerNameInputField : MonoBehaviour
{
	private InputField inputField;

	public static string LocalPlayerName { get; private set; }

	private void Awake()
	{
		inputField = GetComponent<InputField>();
		inputField.onValueChange.AddListener(delegate { HandleValueChanged(); });

		HandleValueChanged();
	}

	private void HandleValueChanged()
	{
		LocalPlayerName = inputField.text;
	}
}
