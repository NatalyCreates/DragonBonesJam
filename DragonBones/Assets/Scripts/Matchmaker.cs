using UnityEngine;

public class Matchmaker : Photon.PunBehaviour
{		
	string debug = "Waiting";

	bool initialized = false;

	private const int numPlayers = 2;

	private PhotonView photonView; // used for RPCs; doesn't synch anything

    void Start()
    {
		PhotonNetwork.autoJoinLobby = true; // used to be default, then they silently changed it, now I hate them
		photonView = GetComponent<PhotonView>();
		PhotonNetwork.ConnectUsingSettings("0.1"); // version number
    }

	public void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		GUILayout.Label(debug);
	}

    public override void OnJoinedLobby()
    {
		// Lobby joined, join random room (on fail event, callback will create a room)
        PhotonNetwork.JoinRandomRoom();
		// TODO: might want to consider proper, latency-conscious matchmaking, but the game's hardly realtime,
		// so this doesn't really matter
    }

	
	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.playerList.Length == numPlayers) { 
			// Everyone's here
			initialized = true;
			if (PhotonNetwork.isMasterClient) {
				// And we're in charge. Generate an RNG seed, and have everyone generate the map
				debug = "Game started; am master";
				photonView.RPC("GenerateMap", Random.Range(int.MinValue, int.MaxValue););
			}
		else
				debug = "Game started; not the master";		
		}
	}

    public void OnPhotonRandomJoinFailed()
    {
		debug = "Creating new room";
        PhotonNetwork.CreateRoom(null);
    }

	public void Update() {
	}
    
}
