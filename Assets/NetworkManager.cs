using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

[System.Serializable]
public class RoomType
{
    public int sceneIndex;
    public int maxPlayer;

    public RoomType(int index, int maxPlayer){
        this.sceneIndex = index;
        this.maxPlayer = maxPlayer;
    }
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public List<RoomType> rooms;
    public GameObject button1;
    public GameObject button2;


    public void Start(){
        rooms = new List<RoomType>();
        rooms.Add(new RoomType(1, 1));
        rooms.Add(new RoomType(1, 5));
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
        button1.SetActive(true);
        button2.SetActive(true);
    }

    public void InitializeRoom(int roomIndex, string name)
    {
        RoomType settings = rooms[roomIndex];

        // LOAD SCENE
        // PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(settings.sceneIndex);

        // CREATE ROOM
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)settings.maxPlayer,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        };
        roomOptions.CustomRoomProperties["stateFile"] = GameMaster.stateFile;

        PhotonNetwork.JoinOrCreateRoom(name, roomOptions, TypedLobby.Default );
        Debug.Log("Join/Create Room");
    }

    public override void OnJoinedRoom()
    {
        // Print out the room name
        Debug.Log("Joined a room: " + PhotonNetwork.CurrentRoom.Name);
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New player joined");
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
