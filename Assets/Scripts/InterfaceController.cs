using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    public GameObject GameOverScreen;
    public GameObject GameWinScreen;
    public GameObject Fader;
    public Text ExitDirectionsText;

    private PlayerController _player;
    private Goal _goal;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _goal = FindObjectOfType<Goal>();
        StartCoroutine(CheckExitDistance());
    }

    void Update()
    {
        if (GameController.Instance.IsGameOver)
        {
            if (!Fader.activeSelf)
                StartCoroutine(StartFade(new Color32(200, 15, 15, 0)));

            if (Input.anyKeyDown)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (GameController.Instance.IsGameVictory)
        {
            if (!Fader.activeSelf)
                StartCoroutine(StartFade(new Color32(200, 200, 15, 0)));

            if (Input.anyKeyDown)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


    }

    IEnumerator StartFade(Color color)
    {
        Fader.SetActive(true);
        var image = Fader.GetComponentInChildren<Image>();
        while (color.a < 1)
        {
            color.a += Time.unscaledDeltaTime*0.5f;
            image.color = color;
            yield return Time.deltaTime;
        }

        if (GameController.Instance.IsGameOver)
            GameOverScreen.SetActive(true);
        else if (GameController.Instance.IsGameVictory)
            GameWinScreen.SetActive(true);
        
        ExitDirectionsText.gameObject.SetActive(false);
    }

    IEnumerator CheckExitDistance()
    {
        while (true)
        {
            float dist = Vector3.SqrMagnitude(_player.transform.position - _goal.transform.position);
            print(dist);
            if (dist < 8)
                ExitDirectionsText.text = "I can see the light";
            else if (dist < 16)
                ExitDirectionsText.text = "Almost there";
            else if (dist < 32)
                ExitDirectionsText.text = "Getting closer";
            else if (dist < 64)
                ExitDirectionsText.text = "A bit more to go";
            else if (dist < 128)
                ExitDirectionsText.text = "Rather far away";
            else if (dist < 256)
                ExitDirectionsText.text = "Just far away";
            else if (dist < 512)
                ExitDirectionsText.text = "Very far";
            else if (dist < 1024)
                ExitDirectionsText.text = "Extremely far";
            else if (dist < 2048)
                ExitDirectionsText.text = "No exit in sight";
            yield return new WaitForSeconds(2);
        }
    }
}
