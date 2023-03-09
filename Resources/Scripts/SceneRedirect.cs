using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script to redirect players to another scene when redirect scene is loaded
public class SceneRedirect : MonoBehaviour
{
    //Called on start
    void Start()
    {
        //Start courtine to wait then load
        StartCoroutine(PausedLoad());
    }

    //Courotine to wait and then load new scene
    IEnumerator PausedLoad()
    {
        //Wait 2.5 seconds
        yield return new WaitForSeconds(2.5f);

        //Load the main lobby scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("lobby");
    }
}
