using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class JoinGameButton : MonoBehaviour
{
    private MatchConnectionInfo matchConnectionInfo;

	[SerializeField]
    private Text buttonText;

    internal void Initialize(MatchConnectionInfo matchInfo, GameListPanel gameListPanel)
    {
		this.matchConnectionInfo = matchInfo;
        buttonText.text = matchConnectionInfo.Name;

        transform.SetParent(gameListPanel.transform);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }

    public void JoinGame()
    {
		FindObjectOfType<PaintballNetworkManager>().JoinGame(matchConnectionInfo);
	}

    private void OnValidate()
    {
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<Text>();
        }
    }
}