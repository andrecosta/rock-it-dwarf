using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    [Header("Controls (WASD)")]
    public Image WImage;
    public Sprite WReleased;
    public Sprite WPressed;
    public Image AImage;
    public Sprite AReleased;
    public Sprite APressed;
    public Image SImage;
    public Sprite SReleased;
    public Sprite SPressed;
    public Image DImage;
    public Sprite DReleased;
    public Sprite DPressed;
    [Header("Controls (Arrows)")]
    public Image UpImage;
    public Sprite UpReleased;
    public Sprite UpPressed;
    public Image LeftImage;
    public Sprite LeftReleased;
    public Sprite LeftPressed;
    public Image DownImage;
    public Sprite DownReleased;
    public Sprite DownPressed;
    public Image RightImage;
    public Sprite RightReleased;
    public Sprite RightPressed;
    [Header("Controls (LThumb)")]
    public Image LThumbImage;
    public Sprite LThumbIdle;
    public Sprite LThumbUp;
    public Sprite LThumbDown;
    public Sprite LThumbLeft;
    public Sprite LThumbRight;
    [Header("Controls (RThumb)")]
    public Image RThumbImage;
    public Sprite RThumbIdle;
    public Sprite RThumbUp;
    public Sprite RThumbDown;
    public Sprite RThumbLeft;
    public Sprite RThumbRight;
    [Header("Controls (Actions)")]
    public Image EscImage;
    public Sprite EscReleased;
    public Sprite EscPressed;
    public Image StartImage;
    public Sprite StartReleased;
    public Sprite StartPressed;
    public Image RImage;
    public Sprite RReleased;
    public Sprite RPressed;
    public Image YImage;
    public Sprite YReleased;
    public Sprite YPressed;
    [Header("Visual")]
    public Text BlinkingText;
    public Image QuitFillBar;

    private GameController _gc;

    void Awake()
    {
    }

    void Start()
    {
        _gc = GameController.Instance;

        // Bind events
        _gc.OnPause += Pause;
        _gc.OnUnpause += Unpause;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            // WASD keys
            WImage.sprite = Input.GetKey(KeyCode.W) ? WPressed : WReleased;
            AImage.sprite = Input.GetKey(KeyCode.A) ? APressed : AReleased;
            SImage.sprite = Input.GetKey(KeyCode.S) ? SPressed : SReleased;
            DImage.sprite = Input.GetKey(KeyCode.D) ? DPressed : DReleased;

            // Arrow keys
            UpImage.sprite = Input.GetKey(KeyCode.UpArrow) ? UpPressed : UpReleased;
            LeftImage.sprite = Input.GetKey(KeyCode.LeftArrow) ? LeftPressed : LeftReleased;
            DownImage.sprite = Input.GetKey(KeyCode.DownArrow) ? DownPressed : DownReleased;
            RightImage.sprite = Input.GetKey(KeyCode.RightArrow) ? RightPressed : RightReleased;

            // Esc key
            EscImage.sprite = Input.GetKey(KeyCode.Escape) ? EscPressed : EscReleased;

            // Start button
            StartImage.sprite = Input.GetKey(KeyCode.JoystickButton7) ? StartPressed : StartReleased;

            // R key
            RImage.sprite = Input.GetKey(KeyCode.R) ? RPressed : RReleased;

            // Y button
            YImage.sprite = Input.GetKey(KeyCode.JoystickButton3) ? YPressed : YReleased;


            // Left thumb image
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (h < 0)
                LThumbImage.sprite = LThumbLeft;
            else if (h > 0)
                LThumbImage.sprite = LThumbRight;
            else if (v < 0)
                LThumbImage.sprite = LThumbDown;
            else if (v > 0)
                LThumbImage.sprite = LThumbUp;
            else
                LThumbImage.sprite = LThumbIdle;

            // Right thumb image
            h = Input.GetAxis("ShootHorizontal");
            v = Input.GetAxis("ShootVertical");
            if (h < 0)
                RThumbImage.sprite = RThumbLeft;
            else if (h > 0)
                RThumbImage.sprite = RThumbRight;
            else if (v < 0)
                RThumbImage.sprite = RThumbDown;
            else if (v > 0)
                RThumbImage.sprite = RThumbUp;
            else
                RThumbImage.sprite = RThumbIdle;

            // "Continue" text blink
            BlinkingText.color = Mathf.Sin(Time.unscaledTime * 20) > 0
                ? new Color(219 / 255f, 211 / 255f, 205 / 255f)
                : new Color(180 / 255f, 147 / 255f, 122 / 255f);

            // "Quit" fill bar
            if (Input.GetButton("Pause"))
            {
                QuitFillBar.fillAmount += Time.unscaledDeltaTime;
                if (QuitFillBar.fillAmount >= 1)
                {
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #endif
                    Application.Quit();
                }
            }
            else
                QuitFillBar.fillAmount -= Time.unscaledDeltaTime;
        }
    }

    void Pause()
    {
        print("Pause");
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    void Unpause()
    {
        print("Unpause");
        Time.timeScale = 1;
        QuitFillBar.fillAmount = 0;
        gameObject.SetActive(false);
    }
}
