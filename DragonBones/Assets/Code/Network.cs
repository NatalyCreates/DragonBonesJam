using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Network : Photon.PunBehaviour
{		
	static public Network instance;

	IList<Incantation> actionsReceived = new List<Incantation>();

	bool otherPlayerIdled = false;

	public bool started = false;

	private const int numPlayers = 2;

    void Start()
    {
		instance = this;

		PhotonNetwork.autoJoinLobby = true; // used to be default, then they silently changed it, now I hate them
		PhotonNetwork.ConnectUsingSettings("0.1"); // version number
    }

    public override void OnJoinedLobby()
    {
		// Lobby joined, join random room (on fail event, callback will create a room)
        PhotonNetwork.JoinRandomRoom();
		// TODO: might want to consider proper, latency-conscious matchmaking, but the game's hardly realtime,
		// so this doesn't really matter
    }
	
	public override void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.playerList.Length == numPlayers) { 
			// Everyone's here
			Basics.assert(!started);
			started = true;

			if (PhotonNetwork.isMasterClient) {
				// And we're in charge. Generate an RNG seed, and have everyone generate the map
				Basics.Log("Game started; am master");
				photonView.RPC("GenerateMap",  PhotonTargets.All, Random.Range(int.MinValue, int.MaxValue));
			}
		else
				Basics.Log ("Game started; not the master");		
		}
	}

    public void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }	   

	[PunRPC]
	public void ReceiveActions(string actions) {
		Basics.assert(waitingForTurn);

		Basics.Log("Receiving actions");

		if (actions == "_NOTHING_") {
			otherPlayerIdled = true;
			return;
		}

		foreach (var descriptor in actions.Split('|')) {
			if (descriptor.Length == 0)
				continue;

			actionsReceived.Add(new Incantation(descriptor));
		}
	}

	public void SendActions() {
		Basics.Log("Sending actions");
		var actions = DragonGame.instance.localPlayer.actionsTaken;
		var descriptors = "";

		if (actions.Count == 0)
			// Special case: we have decided to do nothing
			descriptors = "_NOTHING_";
		else {
			// Serialize actions
			foreach (var action in actions) {
				var descriptor = action.SortaSerialize();

				Basics.assert(!descriptor.Contains("|"));

				descriptors += descriptor + "|";
			}
		}

		photonView.RPC("ReceiveActions",  PhotonTargets.Others, descriptors);
	}

	bool turnTaken {
		get {
			return DragonGame.instance.localPlayer.turnTaken;
		}
		set {
			DragonGame.instance.localPlayer.turnTaken = value;
		}
	}

	public void Update() {
		if (!started) {
			Basics.assert(waitingForTurn); // shouldn't receive turn before game start
			return;
		}
		while (waitingForTurn)
			continue;

		// Have both players acted?
		if (turnTaken && !waitingForTurn) {
			// Yes. First mimic the other player's actions
			Basics.Log("Turn done, replaying foe's actions");
			foreach (var a in actionsReceived)
				DragonGame.instance.ProcessAction(a);

			// Now, start new turn
			Basics.Log("Starting new turn");
			actionsReceived.Clear();
			otherPlayerIdled = false;
			turnTaken = false;
			// TODO: synch error prevention. Send and receive "readyForAction" before setting turnTaken = false
		}
	}

	public bool waitingForTurn {
		get {
			return actionsReceived.Count == 0 && !otherPlayerIdled;
		}
	}
}
