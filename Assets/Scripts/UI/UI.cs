using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelection { get; private set; }
    public UI_GameOver gameOverUI { get; private set; }
    public GameObject pauseUI;

    [SerializeField] private GameObject[] uiElements;

    private void Awake()
    {
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
        gameOverUI = GetComponentInChildren<UI_GameOver>(true);
    }

    private void Start()
    {
        AssignInputsUI();
    }

    public void SwitchTo(GameObject _uiToSwitchOn)
    {
        foreach (var go in uiElements)
        {
            go.SetActive(false);
        }

        _uiToSwitchOn.SetActive(true);
    }


    public void StartTheGame()
    {
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();
    }

    public void QuitTheGame() => Application.Quit();

    public void RestartTheGame() => GameManager.instance.RestartScene();

    public void PauseSwitch()
    {
        bool gamePaused = pauseUI.activeSelf;

        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            TimeManager.instance.ResumeTime();
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            TimeManager.instance.PauseTime();
        }
    }

    public void ShowGameOverUI(string message = "Game Over")
    {
        SwitchTo(gameOverUI.gameObject);
        gameOverUI.ShowGameOverMessage(message);
    }

    private void AssignInputsUI()
    {
        PlayerControls controls = GameManager.instance.player.controls;

        controls.UI.UI_Pause.performed += ctx => PauseSwitch();
    }
}
