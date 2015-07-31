using UnityEngine;

public class Matchmaker : Photon.PunBehaviour
{		
	private const int numPlayers = 2;

    void Start()
    {
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
			if (PhotonNetwork.isMasterClient) {
				// And we're in charge. Generate an RNG seed, and have everyone generate the map
				Ui.instance.debug = "Game started; am master";
				photonView.RPC("GenerateMap",  PhotonTargets.All, Random.Range(int.MinValue, int.MaxValue));
			}
		else
				Ui.instance.debug = "Game started; not the master";		
		}
	}

    public void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }	   
}
