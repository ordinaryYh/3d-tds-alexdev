using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    [Header("Setting")]
    public bool friendlyFire;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();
    }

    public void GameStart()
    {
        LevelGenerator.instance.InitializeGeneration();
        SetDefaultWeaponsForPlayer();
        //任务开始，在LevelGenerator的finish函数当中
    }

    public void RestartScene() =>SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void SetDefaultWeaponsForPlayer()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();
        player.weapon.SetDefaultWeapon(newList);
    }
}
