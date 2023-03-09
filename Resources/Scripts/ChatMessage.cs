using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proelium;
using TMPro;

namespace Proelium
{

    public class ChatMessage : MonoBehaviour
    {

        private void Start()
        {
            GetComponent<TextMeshProUGUI>().color = new Color(GetComponent<TextMeshProUGUI>().color.r, GetComponent<TextMeshProUGUI>().color.g, GetComponent<TextMeshProUGUI>().color.b, 0);
            StartCoroutine(SlowAppear());
        }

        public void SetupItemText(string value, float r, float g, float b)
        {
            this.GetComponent<TextMeshProUGUI>().text = value;
            this.GetComponent<TextMeshProUGUI>().color = new Color(r, g, b);
        }

        public void SetupText(string value, float r, float g, float b)
        {
            this.GetComponent<TextMeshProUGUI>().text = value;
            this.GetComponent<TextMeshProUGUI>().color = new Color(r, g, b);
        }

        IEnumerator SlowAppear()
        {
            GetComponent<TextMeshProUGUI>().color = new Color(GetComponent<TextMeshProUGUI>().color.r, GetComponent<TextMeshProUGUI>().color.g, GetComponent<TextMeshProUGUI>().color.b, 0.0f);

            for (int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(.01f);

                GetComponent<TextMeshProUGUI>().color = new Color(GetComponent<TextMeshProUGUI>().color.r, GetComponent<TextMeshProUGUI>().color.g, GetComponent<TextMeshProUGUI>().color.b, GetComponent<TextMeshProUGUI>().color.a + .01f);
            }

            yield return new WaitForSeconds(4.0f);

            for (int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(.01f);

                GetComponent<TextMeshProUGUI>().color = new Color(GetComponent<TextMeshProUGUI>().color.r, GetComponent<TextMeshProUGUI>().color.g, GetComponent<TextMeshProUGUI>().color.b, GetComponent<TextMeshProUGUI>().color.a - .01f);
            }
        }

    }

}