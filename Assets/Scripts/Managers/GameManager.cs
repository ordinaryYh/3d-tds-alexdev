using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    [Header("Setting")]
    public bool friendlyFire;
    [Space]
    public bool quickStart;

    // [Header("pools")]
    // private ObjectPool<GameObject> bulletPool;
    // public GameObject bulletPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void GameStart()
    {
        SetDefaultWeaponsForPlayer();

        //生成关卡的步骤在选择武器栏目里面，点击next之后，就会生成关卡

        //任务开始，在LevelGenerator的finish函数当中
        //MissionManager.instance.StartMission();
    }

    public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void GameCompleted()
    {
        UI.instance.ShowVictoryScreenUI();
        ControlsManager.instance.controls.Character.Disable();
        player.health.currentHealth += 99999;
    }

    public void GameOver()
    {
        Debug.Log("游戏结束");
        TimeManager.instance.SlowMotionFor(1.5f);
        UI.instance.ShowGameOverUI();
        CameraManager.instance.ChangeCameraDistance(5);
    }

    public void SetDefaultWeaponsForPlayer()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();
        player.weapon.SetDefaultWeapon(newList);
    }
}
