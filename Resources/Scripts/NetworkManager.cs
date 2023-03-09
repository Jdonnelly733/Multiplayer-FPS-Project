using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : PhotonView
{

    /* --- USEFUL METHODS --

    NetworkMethod (int id, string type, string parameters)                                                                                                                                             - Calls RPC to call a method over all clients, id can be used to target a specific network object on those clients, type can be used to indicate the intent of the call, and parameters are used to specificy data for that object

    DestroyOverNetwork (int id)                                                                                                                                                                                    - Calls RPC to destroy a networked object over all clients via passed ID

    DisconnectClient (int id)                                                                                                                                                                                          - Calls method & RPC to disconnect the client who called it from the server and either redirect them to the main menu or close the game. ID is used to specify that player's player object's network ID to destroy their player object upon them leaving

    CreateNetworkedObject(string objectPrefab, (NOT REQUIRED -->) bool specificClientOwned, string owningClientsName, string objectParameters)      - Calls RPC to create an object across all clients. Can create it also for a specific client to own/control

    */

    //World Spawnpoint
    public Transform spawn;

    //List of all objects being synced across network
    public Dictionary<int, NetworkedObject> networkedObjects = new Dictionary<int, NetworkedObject>();

    private void Start()
    {
        //Grab current room's room info
        string[] roomInfo = PhotonNetwork.room.Name.Split('&');

        //Check if we are not the master client in this room
        if (!PhotonNetwork.isMasterClient)
        {
            //If we are not, then request a list of the synced objects from the master client
            photonView.RPC("RequestNetworkedObjects", PhotonTargets.All, PhotonNetwork.playerName);
        }
    }

    //Call a function over the network without calling it via RPC. Takes in the network ID of the networked object being reffered to, the type to decifer what to do with that object, and the parameter in which to pass to that object
    public void NetworkMethod(int id, string type, string parameters = "NONE")
    {
        //Actually call the RPC since this object has a photonView, calls acros all currently connected clients
        RPC("RPC_NetworkMethod", PhotonTargets.All, id, type, parameters);
    }

    //Actual RPC version of "CallNetworkedFunction"
    [PunRPC] public void RPC_NetworkMethod(int id, string type, string parameters = "NONE")
    {
        //Check if a networked object with the passed ID exists
        if (networkedObjects.ContainsKey(id))
        {
            //If so, grab that object's physical in-world object and asign it to the "obj" variable
            GameObject obj = networkedObjects[id].physicalObject;

            //Check what type of call this networked method is
            if (type == "Example")
            {
                //Then do something to the grabbed object based off of the parameters string
            }
        }
        //Else if no networked object of that ID can be found
        else if (id == -1)
        {
            //This if can serve non-object-oriented purposes, so reaching this may not necisarilly be an error
            if (type == "Example")
            {
                //Do some other method or event that does not have an associated object
            }
        }
    }

    //Method to disconect client based on that client's player object's networked object ID
    public void DisconnectClient(int id = -1, bool quitApp = false)
    {
        //Check if there is intent to destroy that player's networked object, ie a valid networked object ID was passed
        if (id > -1)
        {
            //Destroy that player's networked object via RPC
            photonView.RPC("RPC_DestroyOverNetwork", PhotonTargets.All, id);
        }

        //Network to all other clients that our player has left the game
        RPC("RPC_OnClientLeave", PhotonTargets.MasterClient, PhotonNetwork.playerName);

        //Disconect the local leaving client safely
        StartCoroutine(DelayedDisconnect(quitApp));
    }

    //RPC that's called on all clients whenever a client leaves
    [PunRPC] public void RPC_OnClientLeave(string client)
    {
        //This could be used to notify players via a chat that a certain player has left, whose client/local name is passed as the "client" parameter
    }

    //Disconnect from network then leave to redirect scene
    IEnumerator DelayedDisconnect(bool quitApp)
    {
        //Wait one second to let remaining RPCs pass
        yield return new WaitForSeconds(1.0f);

        //Officially disconnect from the PhotonNetwork
        PhotonNetwork.Disconnect();

        //If we do not choose to quit the game
        if (!quitApp)
        {
            //Then load the redirect scene
            SceneManager.LoadScene("redirect");
        }
        //If we do choose to quit the game
        else
        {
            //Then quit the game
            Application.Quit();
        }
    }

    //Method that calls an RPC to destroy a networked object on all client's ends
    public void DestroyOverNetwork(int id)
    {
        //Searches to see if passed ID is valid
        if (id > -1)
        {
            //Calls RPC to destroy id object over all clients
            photonView.RPC("RPC_DestroyOverNetwork", PhotonTargets.All, id);
        }
    }

    //RPC to destroy the object whose network ID was passed if it exists
    [PunRPC] void RPC_DestroyOverNetwork(int id)
    {
        //Check if networked object does not exist
        if (!networkedObjects.ContainsKey(id))
        {
            //If not, log an error on clients who are missing said object
            Debug.LogError("Networked Object Not Found!");
            return;
        }

        //If it does exist, destroy said object's physical object
        Destroy(networkedObjects[id].physicalObject);

        //Then remove it from this client's list of networked objects
        networkedObjects.Remove(id);
    }

    //RPC to request all networked objects specifically from the master client
    [PunRPC] void RequestNetworkedObjects(string clientName)
    {
        //Check if we are the master client
        if (PhotonNetwork.isMasterClient)
        {
            //If so, go through each of out networked objects
            foreach (NetworkedObject objec in networkedObjects.Values)
            {
                //Call RPC to create this specific networked object on requesting client's end including position
                photonView.RPC("RPC_CreateObject", PhotonTargets.All, objec.prefablName, objec.objectId, clientName, "locat&" + objec.physicalObject.gameObject.transform.position.x + "#" + objec.physicalObject.gameObject.transform.position.y + "#" + objec.physicalObject.gameObject.transform.position.z);
            }
        }
    }

    //RPC to create object on a specific clients end
    [PunRPC] void RPC_CreateObject(string prefab, int id, string client, string parameters = "NONE")
    {
        //Debug to print what parameters this client recieved
        print("Received These Parameters: '" + parameters + "'");

        //Check if we are not the player intended to recieve this object creation
        if (PhotonNetwork.playerName != client)
        {
            //If not, return
            return;
        }

        //If so, we need to construct that object. We start by creating a new NetworkedObject
        NetworkedObject obj = new NetworkedObject();

        //Assigning the prefab name
        obj.prefablName = prefab;
        
        //Assigning it its id
        obj.objectId = id;

        //Check if out networked objects already contain this object
        if (networkedObjects.ContainsKey(id))
        {
            //If so, return
            return;
        }

        //If not, physically create that object in the world and assign it to the new objects physical object
        obj.physicalObject = Instantiate(Resources.Load<GameObject>(prefab), spawn.position, Quaternion.identity);

        //Check if the object we created has the ComponentNetworked component
        if (obj.physicalObject.GetComponent<ComponentNetworked>() != null)
        {
            //If so, assign it its network ID
            obj.physicalObject.GetComponent<ComponentNetworked>().networkedID = id;


            //Check and see if we passed any parameters for the object
            if (parameters != "NONE")
            {
                //Object has networked parameters, so split them to read
                string[] pars = parameters.Split('@');

                //Go through each parameter string
                foreach (string stri in pars)
                {
                    //Further split each parameter string into sub data
                    string[] parametersSecondary = stri.Split('&');

                    //Check if the parameter is not the base locat or rot type
                    if (parametersSecondary[0] != "locat" && parametersSecondary[0] != "rot")
                    {
                        //If not, which is it?
                        if (parametersSecondary[0] == "Example")
                        {
                            //Do something to initialize the object based on custom passed parameter
                        }
                    }
                    //If this parameter is a location or rotation
                    else
                    {
                        //Check specifically which one it is
                        if (parametersSecondary[0] == "locat")
                        {
                            //Create an array of strings to disect and grab all of the position values
                            string[] pos = parametersSecondary[1].Split('#');

                            //Assign the position to the newly created object
                            obj.physicalObject.transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                        }
                        //Check specifically which one it is
                        else if (parametersSecondary[0] == "rot")
                        {
                            //Create an array of strings to disect and grab all of the rotation values
                            string[] rot = parametersSecondary[1].Split('#');

                            //Assign the rotation to the newly created object
                            obj.physicalObject.transform.rotation = new Quaternion(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), Quaternion.identity.w);
                        }
                    }
                }
            }
        }

        //Add the newly created networked object to the local clients list of them
        networkedObjects.Add(id, obj);
    }

    //Create an object across all clients, can be specific client owned or just general for all clients
    public void CreateNetworkedObject(string objectPrefab, bool specificClientOwned = false, string owningClientsName = "", string objectParameters = "NONE")
    {
        photonView.RPC("RPC_GenerateObjectKey", PhotonTargets.All, objectPrefab, specificClientOwned, owningClientsName, objectParameters);
    }

    //Generate a specific ID for a new object being created
    [PunRPC] void RPC_GenerateObjectKey(string prefab, bool clientReq = false, string clientName = "", string networkedVariables = "NONE")
    {
        //Check if we are the master client (required when creating a new network ID
        if (PhotonNetwork.isMasterClient)
        {
            //If so, attempt to generate a new number 100 times (in case we hit the same number)
            for (int i = 0; i < 100; i++)
            {
                //Select a random number as the key
                int key = Random.Range(0, 9999999);

                //Check if that key is not already being used
                if (!networkedObjects.ContainsKey(key))
                {
                    //Check if a specific client is requesting this object
                    if (clientReq)
                    {
                        //If so, call an RPC to communicate to that client the new object data
                        photonView.RPC("RPC_CreateNetworkedObjectForSpecificClientToOwn", PhotonTargets.All, key, clientName, prefab, networkedVariables);
                    }
                    else
                    {
                        //Otherwise do so to all clients normally
                        photonView.RPC("RPC_CreateNetworkedObjectForAll", PhotonTargets.All, key, prefab, networkedVariables);
                    }

                    return;
                }
            }
        }
    }

    //RPC to recieve a generated ID on all clients
    [PunRPC] void RPC_CreateNetworkedObjectForAll(int val, string prefab, string parameters)
    {
        //Create on all clients now, start by creating a new NetworkedObject entry
        NetworkedObject obj = new NetworkedObject();

        //Assign the passed prefab name
        obj.prefablName = prefab;

        //Assign it its networked ID
        obj.objectId = val;

        //Instantiate and assign it its physical object
        obj.physicalObject = Instantiate(Resources.Load<GameObject>(prefab), spawn.position, Quaternion.identity);

        //Check if that physical object has the ComponentNetworked script
        if (obj.physicalObject.GetComponent<ComponentNetworked>() != null)
        {
            //If so, assign it its value
            obj.physicalObject.GetComponent<ComponentNetworked>().networkedID = val;

            //Check and see if any parameters were passed
            if (parameters != "NONE")
            {
                //Object has networked variables, so split to disect them
                string[] pars = parameters.Split('@');

                //Go through each passed parameter
                foreach (string stri in pars)
                {
                    //Further split each of these
                    string[] parametersSecondary = stri.Split('&');

                    //Check if they do not involve location or rotation
                    if (parametersSecondary[0] != "locat" && parametersSecondary[0] != "rot")
                    {
                        //If not, perfom some type of special action on the object
                        if (parametersSecondary[0] == "Example")
                        {
                            //Can be used to perform certain methods on objects
                        }
                    }
                    //Otherwise if it is a location or rotation
                    else
                    {
                        //Check specifically which of the two
                        if (parametersSecondary[0] == "locat")
                        {
                            //Further disect location string to grab the position values
                            string[] pos = parametersSecondary[1].Split('#');

                            //Assign the object that position
                            obj.physicalObject.transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                        }
                        //If its not location, it's rotation
                        else if (parametersSecondary[0] == "rot")
                        {
                            //Further disect location string to grab the rotation values
                            string[] rot = parametersSecondary[1].Split('#');

                            //Assign the object that rotation
                            obj.physicalObject.transform.rotation = new Quaternion(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), Quaternion.identity.w);
                        }
                    }
                }
            }
        }

        //Add newly created object to networked objects list
        networkedObjects.Add(val, obj);
    }


    //RPC to create a network object on all clients, but for the client who requested it it tells on that client that the network component is theirs
    [PunRPC] void RPC_CreateNetworkedObjectForSpecificClientToOwn(int val, string client, string prefab, string parameters)
    {
        //Check if we are the designated client
        if (PhotonNetwork.playerName == client)
        {
            //Requested Client, create new networked object on that specific client
            NetworkedObject obj = new NetworkedObject();

            //Set prefab name
            obj.prefablName = prefab;

            //Give it its networked id
            obj.objectId = val;

            //Create and assign it its physical object
            obj.physicalObject = Instantiate(Resources.Load<GameObject>(prefab), spawn.position, Quaternion.identity);

            //Check if that physical object has a ComponmentNetworked script
            if (obj.physicalObject.GetComponent<ComponentNetworked>() != null)
            {
                //If the object has the ComponentNetworked script attached, assign THIS client to own it and have control over it
                obj.physicalObject.GetComponent<ComponentNetworked>().isThisClients = true;

                //Also, assign this object its network ID
                obj.physicalObject.GetComponent<ComponentNetworked>().networkedID = val;

                //Check if there were any parameters passed
                if (parameters != "NONE")
                {
                    //Object has networked variables, so split to disect
                    string[] pars = parameters.Split('@');

                    //Go through each passed parameter
                    foreach (string stri in pars)
                    {
                        //Further split each of these string parameters to get further information
                        string[] parametersSecondary = stri.Split('&');

                        //Check if the currently checking parameter is just a location or rotation
                        if (parametersSecondary[0] != "locat" && parametersSecondary[0] != "rot")
                        {
                            //If not, we are here. This can be used to pass custom variables or data for a specific object
                            if (parametersSecondary[0] == "Example")
                            {
                                //Do something specific
                            }
                        }
                        //If the parameter we are checking IS a location or rotation
                        else
                        {
                            //If location
                            if (parametersSecondary[0] == "locat")
                            {
                                //Then disect string for each postion
                                string[] pos = parametersSecondary[1].Split('#');

                                //And assign it to the physical object
                                obj.physicalObject.transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                            }
                            //If rotation
                            else if (parametersSecondary[0] == "rot")
                            {
                                //Then disect string for each postion
                                string[] rot = parametersSecondary[1].Split('#');

                                //And assign it to the physical object
                                obj.physicalObject.transform.rotation = new Quaternion(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), Quaternion.identity.w);
                            }
                        }
                    }
                }
            }

            //Add the newly created object to the networkedObject list
            networkedObjects.Add(val, obj);
        }
        else
        {
            //Other clients, the ones who DID NOT request this object
            //Start by making a new networkedObject
            NetworkedObject obj = new NetworkedObject();

            //Give it its prefab name
            obj.prefablName = prefab;

            //Assign it its network ID
            obj.objectId = val;

            //And create + assign it its physical object
            obj.physicalObject = Instantiate(Resources.Load<GameObject>(prefab), spawn.position, Quaternion.identity);

            //Check if it has a networking component
            if (obj.physicalObject.GetComponent<ComponentNetworked>() != null)
            {
                //If so, give it its networkID
                obj.physicalObject.GetComponent<ComponentNetworked>().networkedID = val;

                //Check for parameters
                if (parameters != "NONE")
                {
                    //Object has networked parameters, so split them to disect
                    string[] pars = parameters.Split('@');

                    //Go through each of them
                    foreach (string stri in pars)
                    {
                        //Further split each for more information
                        string[] parametersSecondary = stri.Split('&');

                        //Check if the parameter is something other than location or rotation
                        if (parametersSecondary[0] != "locat" && parametersSecondary[0] != "rot")
                        {
                            //If so, we are here. This can be used to pass custom variables or data for a specific object
                            if (parametersSecondary[0] == "Example")
                            {
                                //Do something specific
                            }
                        }
                        //If it is a location or rotation, then
                        else
                        {
                            //Check specifically which it is
                            if (parametersSecondary[0] == "locat")
                            {
                                //Further split that string for specific position data
                                string[] pos = parametersSecondary[1].Split('#');

                                //And assign it to the physical object
                                obj.physicalObject.transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                            }
                            //Check specifically which it is
                            else if (parametersSecondary[0] == "rot")
                            {
                                //Further split that string for specific rotation data
                                string[] rot = parametersSecondary[1].Split('#');

                                //And assign it to the physical object
                                obj.physicalObject.transform.rotation = new Quaternion(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), Quaternion.identity.w);
                            }
                        }
                    }
                }
            }
            //Add it to the list of networked objects
            networkedObjects.Add(val, obj);
        }
    }

}