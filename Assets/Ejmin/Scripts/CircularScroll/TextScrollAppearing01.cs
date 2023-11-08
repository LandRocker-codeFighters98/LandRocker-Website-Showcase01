using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextScrollAppearing01 : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    [SerializeField] string[] contents;
    string textToWrite;
    [SerializeField] private float timePerCharacter;
    private int characterIndex;
    private float timer;


    public void WriteText(int indexPassed)
    {
        textToWrite = contents[indexPassed];
        characterIndex = 0;
        StopAllCoroutines();
        if (textComponent)
        {
            textComponent.text = "";
        }
        StartCoroutine(WriteTextsPerCharacter());
    }

    private IEnumerator WriteTextsPerCharacter()
    {
        if (textComponent)
        {
            while (characterIndex < textToWrite.Length)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    timer += timePerCharacter;
                    ++characterIndex;
                    textComponent.text = textToWrite.Substring(0, characterIndex);
                }

                yield return null;
            }
        }
    }
}
