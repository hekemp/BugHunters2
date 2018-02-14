using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public static class GameListController
{
	private static HashSet<MatchConnectionInfo> matchmakerMatches = new HashSet<MatchConnectionInfo>();
	private static HashSet<MatchConnectionInfo> lanMatches = new HashSet<MatchConnectionInfo>();
	
	private static IEnumerable<MatchConnectionInfo> AllMatches()
	{
		return lanMatches.Concat(matchmakerMatches);
	}

	public static event Action<IEnumerable<MatchConnectionInfo>> OnMatchesChanged = delegate { };

	public static void AddMatchmakerMatches(IEnumerable<MatchInfoSnapshot> matches)
	{
		if (ContainsNewMatches(matches) ||
			MatchesWereRemoved(matches))
		{
			foreach (var match in matches)
				matchmakerMatches.Add(new MatchConnectionInfo(match.name, match.directConnectInfos[0].publicAddress, match.networkId));

			OnMatchesChanged(AllMatches());
		}
	}

	private static bool ContainsNewMatches(IEnumerable<MatchInfoSnapshot> matchesToCheck)
	{
		foreach(var match in matchesToCheck)
		{
			if (matchmakerMatches.Any(t => t.NetworkId == match.networkId) == false)
				return true;
		}
		return false;
	}

	private static bool MatchesWereRemoved(IEnumerable<MatchInfoSnapshot> matchesToCheck)
	{
		foreach (var match in matchmakerMatches)
		{
			if (matchesToCheck.Any(t => t.networkId == match.NetworkId) == false)
				return true;
		}
		return false;
	}

	public static void AddLanMatches(IEnumerable<LanConnectionInfo> matchesToAdd)
	{
		lanMatches.Clear();

		foreach (var match in matchesToAdd)
		{
			lanMatches.Add(new MatchConnectionInfo(match.name, match.ipAddress, NetworkID.Invalid));
		}

		OnMatchesChanged(AllMatches());
	}
}

public struct MatchConnectionInfo
{
	public MatchConnectionInfo(string name, string address, NetworkID networkId)
	{
		NetworkId = networkId;
		Name = name;
		Address = address;
	}
	public NetworkID NetworkId { get; private set; }
	public string Address { get; private set; }
	public string Name { get; private set; }

	public bool IsLan { get { return this.NetworkId == NetworkID.Invalid; } }
}