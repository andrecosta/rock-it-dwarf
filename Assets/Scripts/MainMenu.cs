using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text AnyKeyText;

    void Start()
    {

    }

    void Update()
    {
        // "Continue" text blink
        AnyKeyText.color = Mathf.Sin(Time.unscaledTime * 10) > 0
            ? new Color(219 / 255f, 211 / 255f, 205 / 255f)
            : new Color(180 / 255f, 147 / 255f, 122 / 255f);
    }
}
