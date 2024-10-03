using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    //Enemies
    private List<Enemy> enemyList;

    [SerializeField] private NavMeshSurface navMeshSurface;
    [Space]
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private List<Transform> levelParts;
    private List<Transform> currentLevelParts;
    private List<Transform> generatedLevelParts = new List<Transform>();

    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;

    [Space]
    [SerializeField] private float generationCooldown;
    private float cooldownTimer;
    private bool generationOver;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enemyList = new List<Enemy>();
        defaultSnapPoint = nextSnapPoint;
        InitializeGeneration();
    }

    private void Update()
    {
        if (generationOver == true)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0)
        {
            //每次生成，列表都会减少一个，所有都生成完后，就生成finish的levelpart
            //然后就结束生成
            if (currentLevelParts.Count > 0)
            {
                cooldownTimer = generationCooldown;
                GenerateNextLevelPart();
            }
            else if (generationOver == false)
            {
                FinishGeneration();
            }
        }
    }

    //生成最后一个levelpart，这样就停止生成关卡了
    private void FinishGeneration()
    {
        generationOver = true;
        GenerateNextLevelPart();

        //这一步就是bake操作，这样可以避免手动进行bake
        navMeshSurface.BuildNavMesh();

        foreach (Enemy enemy in enemyList)
        {
            enemy.transform.parent = null;
            enemy.gameObject.SetActive(true);
        }
    }

    [ContextMenu("Create next level part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = null;

        if (generationOver)
            newPart = Instantiate(lastLevelPart);
        else
            newPart = Instantiate(ChooseRandomPart());

        //保存在这个列表中
        generatedLevelParts.Add(newPart);

        LevelPart levelPart = newPart.GetComponent<LevelPart>();
        levelPart.SnapAndAlignPartTo(nextSnapPoint);

        if (levelPart.IntersectionDetected())
        {
            InitializeGeneration();
        }

        nextSnapPoint = levelPart.GetExitPoint();
        enemyList.AddRange(levelPart.MyEnemies());
    }

    [ContextMenu("Restart generation")]
    private void InitializeGeneration()
    {
        nextSnapPoint = defaultSnapPoint;
        generationOver = false;
        currentLevelParts = new List<Transform>(levelParts);
        DestroyOldLevelPartAndEnemies();
    }

    private void DestroyOldLevelPartAndEnemies()
    {
        foreach (Enemy enemy in enemyList)
        {
            Destroy(enemy.gameObject);
        }

        foreach (var t in generatedLevelParts)
        {
            Destroy(t.gameObject);
        }

        generatedLevelParts = new List<Transform>();
        enemyList = new List<Enemy>();
    }

    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, currentLevelParts.Count);

        Transform choosenPart = currentLevelParts[randomIndex];

        currentLevelParts.RemoveAt(randomIndex);

        return choosenPart;
    }

    public Enemy GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemyList.Count);

        return enemyList[randomIndex];
    }
}
