using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proelium;
using TMPro;
using UnityEngine.UI;

//Script to store server inforamtion and display to the player
public class ServerSlot : Photon.PunBehaviour
{
    //Text object to display the title of the server
    public TextMeshProUGUI textServerName;

    //Text object to display the player count of the server
    public TextMeshProUGUI textPlayerCount;

    //Gameobject to block the UI elements
    public GameObject UIBlock;

    //Room info data of current room
    public RoomInfo currentRoom;

    //Code to join/accsess room (NOT A PASSWORD!)
    public string joinCode;

    //Method to intially setup UI elements given the server's info
    public void InitializeGUI(RoomInfo serverInfo)
    {
        //Key = Server Name, SecondaryKey = Current Player Count, TershiaryKey = Max Player Count
        //Check if the server name text object exists
        if (textServerName != null)
        {
            //Set the join code
            joinCode = serverInfo.name;

            //Gather the data from the name of the server
            string[] data = serverInfo.name.Split('&');

            //Generate a string for the name to display
            textServerName.text = data[0];
        }

        //Check if the player count text exists
        if (textPlayerCount != null)
        {
            //If so, set the player count text to the player count
            textPlayerCount.text = serverInfo.PlayerCount + "/" + serverInfo.MaxPlayers;
        }

        //Set the current room object to the passed server information
        currentRoom = serverInfo;
    }

    //Method called when players presses the button to join this server
    public void JoinServer()
    {
        //Check if there is room in this server
        if (currentRoom.PlayerCount == currentRoom.MaxPlayers)
        {
            //If not, return
            return;
        }

        //If there is room, block the user from joining other servers via UI block
        UIBlock.SetActive(true);

        //Actually join the room via code
        PhotonNetwork.JoinRoom(joinCode);
    }

    //Method called when our client joins a given room
    private void OnJoinedRoom()
    {
        //Split the data of the room to analyze, could be used to join a specific map depending on room data
        string[] roomDat = PhotonNetwork.room.Name.Split('&');

        //Example of how this could be used
        if (roomDat[1] == "Example")
        {
            //Load via Photon the specific map passed in the room data
            PhotonNetwork.LoadLevel(roomDat[2]);
        }
    }

}