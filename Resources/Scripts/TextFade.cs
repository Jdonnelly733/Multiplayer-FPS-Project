using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextFade : MonoBehaviour
{
    TextMeshProUGUI text;
    [HideInInspector] public bool onlyIn;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        StartCoroutine(AlterText());
    }

    IEnumerator AlterText()
    {
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.025f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + 0.01f) ;
        }

        yield return new WaitForSeconds(1.0f);

        if (onlyIn)
        {
            yield break;
        }

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.025f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - 0.01f);
        }

        Destroy(this.gameObject);
    }
}
