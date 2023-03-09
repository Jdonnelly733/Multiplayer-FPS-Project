using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proelium;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Proelium
{

    public class buttonDisconnect : Photon.MonoBehaviour
    {

        public Sprite currentSprite;
        public Sprite hoverSprite;
        public Image buttonImage;

        public GameObject audioObject;

        public AudioClip hoverSound;
        public AudioClip clickSound;

        public bool hasAnimation;

        public UnityEvent onButtonPress;

        public GameObject wholePlayer;

        public Texture2D hoverTexture;
        public Texture2D unHoverTexture;

        public void buttonHover()
        {
            Cursor.SetCursor(hoverTexture, new Vector2(0, 0), CursorMode.Auto);
            if (hoverSound != null && audioObject != null && audioObject.GetComponent<AudioSource>() != null)
            {
                GameObject newAudio = Instantiate(audioObject, this.transform);
                AudioSource buttonSound = newAudio.GetComponent<AudioSource>();

                buttonSound.clip = hoverSound;
                buttonSound.Play();
            }

            if (hoverSprite != null)
            {
                buttonImage.sprite = hoverSprite;
            }

        }

        public void buttonUnHover()
        {
            Cursor.SetCursor(unHoverTexture, new Vector2(0, 0), CursorMode.Auto);
            if (currentSprite != null)
            {
                buttonImage.sprite = currentSprite;
            }
        }

        public void buttonPressed()
        {

            if (clickSound != null && audioObject != null && audioObject.GetComponent<AudioSource>() != null)
            {
                GameObject newAudio = Instantiate(audioObject, this.transform);
                AudioSource buttonSound = newAudio.GetComponent<AudioSource>();

                buttonSound.clip = clickSound;
                buttonSound.Play();
            }

            if (onButtonPress != null)
            {
                onButtonPress.Invoke();
            }
            
            StartCoroutine(WaitLeave());

        }

        IEnumerator WaitLeave()
        {
            yield return new WaitForSeconds(.25f);
        }

    }

}