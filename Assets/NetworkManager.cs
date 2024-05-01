using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class RoomType
{
    public string name;
    public int sceneIndex;
    public int maxPlayer;

    public RoomType(string name, int index, int maxPlayer){
        this.name = name;
        this.sceneIndex = index;
        this.maxPlayer = maxPlayer;
    }
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public List<RoomType> rooms;

    public void Start(){
        rooms = new List<RoomType>();
        rooms.Add(new RoomType("Singleplayer", 1, 1));
        rooms.Add(new RoomType("Multiplayer", 1, 2));
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Trying to connect");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined Lobby");
    }

    public void InitializeRoom(int roomIndex)
    {
        RoomType settings = rooms[roomIndex];

        // LOAD SCENE
        // PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(settings.sceneIndex);

        // CREATE ROOM
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)settings.maxPlayer;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom(settings.name, roomOptions, TypedLobby.Default );
        Debug.Log("Join/Create Room");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room.");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New player joined");
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
