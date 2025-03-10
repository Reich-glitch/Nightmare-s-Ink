using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public Image blackoutScreen;
    public GameObject continuePrompt;

    private string[] storyLines = {
        "Nightmares Ink is a 2D platformer where players control Pen...",
        "A young artist trapped in a dream turned nightmare...",
        "Her once-friendly doodles have come to life as terrifying scribble creatures...",
        "To escape, she must collect magical erasers to help her wake up,",
        "And defeat the monstrous figures trying to trap her in the nightmare...",
        "",
        "But what if...",
        "She never wakes up?"
    };

    void Start()
    {
        blackoutScreen.canvasRenderer.SetAlpha(1f); // Ensure full black
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        blackoutScreen.gameObject.SetActive(true);
        continuePrompt.SetActive(false);
        storyText.text = "";

        yield return new WaitForSeconds(1f);
        blackoutScreen.CrossFadeAlpha(0, 2f, false); // Smooth fade out

        yield return new WaitForSeconds(2f);

        foreach (string line in storyLines)
        {
            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSeconds(1.5f);
        }

        continuePrompt.SetActive(true);
    }

    IEnumerator TypeText(string line)
    {
        storyText.text = "";
        foreach (char c in line)
        {
            storyText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void Update()
    {
        if (continuePrompt.activeSelf && Input.anyKeyDown)
        {
            Debug.Log("Key Pressed! Loading MainMenu...");
            SceneManager.LoadScene("SampleScene"); // Ensure this scene name is in Build Settings
        }
    }
}
