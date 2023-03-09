using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proelium;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Proelium
{

    public class LobbyManager : Photon.PunBehaviour
    {

        public string i_gameVersion = "game";
        public string i_selectedName = "null";
        public bool i_singleplayer;

        public TextMeshProUGUI textCharacterName;
        public TextMeshProUGUI textNoOneOnline;
        public TextMeshProUGUI textWorldSelection;

        public TMP_InputField inputfieldServerName;
        public TMP_InputField inputfieldName;
        public TMP_InputField inputfieldServerMaxPlayer;
        public TMP_InputField inputfieldServerCode;

        public TMP_Dropdown dropdownGamemode;
        public TMP_Dropdown dropdownJoinGamemode;

        public TMP_Dropdown dropdownMaps;
        public TMP_Dropdown dropdownMapsJoin;

        public GameObject scrollviewServerListing;
        public GameObject panelLoading;
        public GameObject panelConnectionError;
        public GameObject panelCreateServer;
        public GameObject panelJoinViaCode;
        public GameObject panelServerView;
        public GameObject panelSelectName;

        public GameObject panelCreating;
        public GameObject panelJoining;

        public bool loadedSucsesfully;

        private void Awake()
        {
            if (!i_singleplayer)
            {
                panelCreating.SetActive(false);
                panelJoining.SetActive(false);
            }

            //PhotonNetwork.JoinLobby();
            if (i_singleplayer)
            {
                PhotonNetwork.ConnectUsingSettings(i_gameVersion);
            }
            else
            {

                panelLoading.SetActive(true);
                textNoOneOnline.gameObject.SetActive(false);
                panelConnectionError.SetActive(false);
                PhotonNetwork.ConnectUsingSettings(i_gameVersion);

                StartCoroutine(ConnectionPing());
            }
        }

        public void RetryConnection()
        {
            SceneManager.LoadScene("lobbyScene");
        }

        public override void OnReceivedRoomListUpdate()
        {
            base.OnReceivedRoomListUpdate();

            foreach (Transform child in scrollviewServerListing.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            RoomInfo[] returnedServers = GetActiveServers();

            if (returnedServers.Length <= 0)
            {
                textNoOneOnline.gameObject.SetActive(true);
            }
            else
            {
                textNoOneOnline.gameObject.SetActive(false);
                for (int i = 0; i < returnedServers.Length; i++)
                {
                    GameObject newServerListing = Instantiate(Resources.Load<GameObject>("Prefabs/a_defaultServerSlot"), scrollviewServerListing.transform);

                    newServerListing.GetComponent<ServerSlot>().UIBlock = panelJoining;
                    newServerListing.GetComponent<ServerSlot>().InitializeGUI(returnedServers[i]);
                }
            }
        }

        IEnumerator ConnectionPing()
        {
            yield return new WaitForSeconds(5.0f);

            if (!loadedSucsesfully)
            {
                panelConnectionError.SetActive(true);
                panelLoading.SetActive(false);
            }
        }

        public void SetUsername()
        {
            if (inputfieldName.text.Length >= 3)
            {
                i_selectedName = inputfieldName.text;
                textCharacterName.text = inputfieldName.text;
                PhotonNetwork.playerName = i_selectedName;
                SetGUIActiveStatus(0);
            }
        }

        public void SetGUIActiveStatus(int val)
        {
            if (val == -1)
            {
                //Select Name Screen
                panelSelectName.SetActive(true);
                panelCreateServer.SetActive(false);
                panelJoinViaCode.SetActive(false);
                panelServerView.SetActive(false);
            }
            if (val == 0)
            {
                //Return to main server menu
                panelSelectName.SetActive(false);
                panelCreateServer.SetActive(false);
                panelJoinViaCode.SetActive(false);
                panelServerView.SetActive(true);
            }
            else if (val == 1)
            {
                //Go to creation menu
                panelSelectName.SetActive(false);
                panelCreateServer.SetActive(true);
                panelJoinViaCode.SetActive(false);
                panelServerView.SetActive(false);
                OnGamemodeSelected_Create();
            }
            else if (val == 2)
            {
                //Go to join via code menu
                panelSelectName.SetActive(false);
                panelCreateServer.SetActive(false);
                panelJoinViaCode.SetActive(true);
                panelServerView.SetActive(false);
                OnGamemodeSelected_Join();
            }
        }

        private void OnConnectedToMaster()
        {
            loadedSucsesfully = true;

            if (i_singleplayer)
            {
                return;
            }

            SetGUIActiveStatus(-1);
            PhotonNetwork.JoinLobby();
            panelLoading.SetActive(false);
        }

        private RoomInfo[] GetActiveServers()
        {
            RoomInfo[] Rooms = PhotonNetwork.GetRoomList();

            return Rooms;
        }

        private void OnJoinedLobby()
        {

        }

        private void OnDisconnectedFromPhoton()
        {
            //Debug.Log("Lost Connection To Photon");
        }

        public void Start()
        {
            if (i_singleplayer == true)
            {
                PhotonNetwork.offlineMode = true;
                CreateServer();
            }
            else
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

                //ADD DATA
                TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
                newOption.text = "Example";
                options.Add(newOption);

                dropdownGamemode.ClearOptions();
                dropdownGamemode.AddOptions(options);

                dropdownJoinGamemode.ClearOptions();
                dropdownJoinGamemode.AddOptions(options);
            }
        }

        public Texture2D hoverTexture;
        public Texture2D unHoverTexture;

        public void SetCursorType(int id)
        {
            if (id == 0)
            {
                Cursor.SetCursor(unHoverTexture, new Vector2(0, 0), CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(hoverTexture, new Vector2(0, 0), CursorMode.Auto);
            }
        }

        public void JoinRandom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void CreateServer()
        {

            if (!i_singleplayer)
            {
                if (Int64.Parse(inputfieldServerMaxPlayer.text) > 0 && inputfieldServerName.text.Length >= 2)
                {
                    if (!i_singleplayer)
                    {
                        panelCreating.SetActive(true);
                        panelJoining.SetActive(false);
                    }
                    RoomOptions roomop = new RoomOptions
                    {
                        IsOpen = true,
                        IsVisible = true,
                        MaxPlayers = (byte)(Int64.Parse(inputfieldServerMaxPlayer.text))
                    };

                    if (dropdownJoinGamemode.options[dropdownGamemode.value].text == "1v1")
                    {
                        roomop.MaxPlayers = (byte)2;
                    }

                    PhotonNetwork.CreateRoom(inputfieldServerName.text.ToString() + "&" + dropdownGamemode.options[dropdownGamemode.value].text + "&" + dropdownMaps.options[dropdownMaps.value].text, roomop, TypedLobby.Default);
                    //PhotonNetwork.LoadLevel("multTest");
                }
            }
            else
            {
                if (!i_singleplayer)
                {
                    panelCreating.SetActive(true);
                    panelJoining.SetActive(false);
                }
                PhotonNetwork.CreateRoom("server.localSingleplayer");
                //PhotonNetwork.LoadLevel("multTest");
            }
        }

        public void JoinRoom()
        {
            panelCreating.SetActive(false);
            panelJoining.SetActive(true);
            RoomOptions roomOptions = new RoomOptions();
            //roomOptions.MaxPlayers = (byte)lobbyMaxPlayers;

            PhotonNetwork.JoinRoom(inputfieldServerCode.text + "&" + dropdownJoinGamemode.options[dropdownGamemode.value].text + "&" + dropdownMapsJoin.options[dropdownMapsJoin.value].text);
        }

        private void OnJoinedRoom()
        {
            string[] roomDat = PhotonNetwork.room.Name.Split('&');
            print("Server Name: " + PhotonNetwork.room.Name);

            string levelToLoad = roomDat[2];

            if (roomDat[1] == "Sandbox")
            {

            }
            else if (roomDat[1] == "1v1")
            {

            }
            PhotonNetwork.LoadLevel(levelToLoad);
        }

        public void OnGamemodeSelected_Create()
        {
            dropdownGamemode.interactable = true;

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            string gamemodeName = dropdownGamemode.options[dropdownGamemode.value].text;

            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = "Example";
            options.Add(newOption);

            dropdownMaps.ClearOptions();
            dropdownMaps.AddOptions(options);
            dropdownMaps.interactable = true;

            inputfieldServerMaxPlayer.interactable = true;
            inputfieldServerMaxPlayer.text = 0.ToString();
        }

        public void OnGamemodeSelected_Join()
        {
            dropdownGamemode.interactable = true;

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = "Example";
            options.Add(newOption);

            dropdownMapsJoin.ClearOptions();
            dropdownMapsJoin.AddOptions(options);
            dropdownMapsJoin.interactable = true;
        }

    }
    
}