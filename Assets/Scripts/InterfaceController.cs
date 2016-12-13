using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    public GameObject GameOverscreen;
    public GameObject Fader;

    void Start()
    {
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

        GameOverscreen.SetActive(true);
    }
}
