using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;

public class GameListPanel : MonoBehaviour
{
    [SerializeField]
    private JoinGameButton joinGameButtonPrefab;

	private void Awake()
	{
		GameListController.OnMatchesChanged += SetGameList;
	}

	private void OnDestroy()
	{
		GameListController.OnMatchesChanged -= SetGameList;
	}

	public void SetGameList(IEnumerable<MatchConnectionInfo> matchInfos)
    {
        ClearExistingButtons();

        string text = string.Empty;
        foreach (var matchInfo in matchInfos)
        {
            var button = Instantiate(joinGameButtonPrefab);
            button.Initialize(matchInfo, this);
        }
    }

    private void ClearExistingButtons()
    {
        var buttons = GetComponentsInChildren<JoinGameButton>();
        foreach(var button in buttons)
        {
            Destroy(button.gameObject);
        }
    }
}