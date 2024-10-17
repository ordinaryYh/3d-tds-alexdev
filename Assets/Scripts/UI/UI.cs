using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelection { get; private set; }
    public UI_GameOver gameOverUI { get; private set; }
    public UI_Settings settingsUI { get; private set; }
    public GameObject victoryScreenUI;
    public GameObject pauseUI;

    [SerializeField] private GameObject[] uiElements;

    [Header("Fade Image")]
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
        gameOverUI = GetComponentInChildren<UI_GameOver>(true);
        settingsUI = GetComponentInChildren<UI_Settings>(true);
    }

    private void Start()
    {
        AssignInputsUI();

        StartCoroutine(ChangeImageAlpha(0, 1.5f, null));

        //这个代码仅仅为了测试方便，打包前要注释掉
        if (GameManager.instance.quickStart)
        {
            LevelGenerator.instance.InitializeGeneration();
            StartTheGame();
        }
    }

    public void SwitchTo(GameObject _uiToSwitchOn)
    {
        foreach (var go in uiElements)
        {
            go.SetActive(false);
        }

        _uiToSwitchOn.SetActive(true);

        if (_uiToSwitchOn == settingsUI.gameObject)
            settingsUI.LoadValues();
    }


    public void StartTheGame() => StartCoroutine(StartGameSequence());

    public void QuitTheGame() => Application.Quit();
    public void StartLevelGeneration() => LevelGenerator.instance.InitializeGeneration();

    public void RestartTheGame()
    {
        TimeManager.instance.ResumeTime();
        StartCoroutine(ChangeImageAlpha(1, 1f, GameManager.instance.RestartScene));
        //GameManager.instance.RestartScene();
    }


    public void PauseSwitch()
    {
        bool gamePaused = pauseUI.activeSelf;

        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            TimeManager.instance.ResumeTime();
            Cursor.visible = false;
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            TimeManager.instance.PauseTime();
            Cursor.visible = true;
        }
    }

    public void ShowGameOverUI(string message = "Game Over")
    {
        SwitchTo(gameOverUI.gameObject);
        gameOverUI.ShowGameOverMessage(message);
    }

    public void ShowVictoryScreenUI()
    {
        StartCoroutine(ChangeImageAlpha(1, 1.5f, SwitchToVictoryScreenUI));
    }

    private void SwitchToVictoryScreenUI()
    {
        SwitchTo(victoryScreenUI);

        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;
    }

    private void AssignInputsUI()
    {
        PlayerControls controls = GameManager.instance.player.controls;

        controls.UI.UI_Pause.performed += ctx => PauseSwitch();
    }

    private IEnumerator StartGameSequence()
    {
        bool quickStart = GameManager.instance.quickStart;

        if (quickStart == false)
        {
            fadeImage.color = Color.black;
            StartCoroutine(ChangeImageAlpha(1, 1, null));
            yield return new WaitForSeconds(1);
        }


        yield return null;
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();

        if (quickStart)
            StartCoroutine(ChangeImageAlpha(0, 0.1f, null));
        else
            StartCoroutine(ChangeImageAlpha(0, 1, null));

    }

    private IEnumerator ChangeImageAlpha(float _targetAlpha, float _duration, System.Action onComplete)
    {
        float time = 0;

        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;

        while (time <= _duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, _targetAlpha, time / _duration);

            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            //这个代表这帧暂停，下一帧又开始，用于确保alpha通道的改变是逐渐的而不是瞬时的
            //这种用法很常见
            yield return null;
        }

        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, _targetAlpha);

        onComplete?.Invoke();
    }

    [ContextMenu("Assign Audio To Button")]
    public void AssignAudioListenersToButtons()
    {
        UI_Button[] buttons = FindObjectsOfType<UI_Button>(true);

        foreach (var button in buttons)
        {
            button.AssighnAudioSource();
        }
    }
}
