using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script to hold the data of each networked object in a large list on the NetworkManager
public class NetworkedObject : MonoBehaviour
{
    //String to store the name of this object's prefab
    public string prefablName;

    //Integer to store this object's network ID
    public int objectId;
    
    //GameObject to store the actual physical in-world version of this object
    public GameObject physicalObject;

}
