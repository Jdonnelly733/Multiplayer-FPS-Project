using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Script used on all networked objects
public class ComponentNetworked : MonoBehaviour
{
    //Local objects network ID across all clients
    public int networkedID;

    //Boolean to see if this client has control over this object
    public bool isThisClients;
}
