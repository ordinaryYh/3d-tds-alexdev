using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private int poolSize = 5;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary =
        new Dictionary<GameObject, Queue<GameObject>>();

    [Header("To Initialize")]
    [SerializeField] private GameObject weaponPickup;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeNewPool(weaponPickup);
    }


    public GameObject GetObject(GameObject _prefab)
    {
        if (poolDictionary.ContainsKey(_prefab) == false)
        {
            InitializeNewPool(_prefab);
        }

        if (poolDictionary[_prefab].Count == 0)
        {
            CreateNewObject(_prefab);
        }

        GameObject objectToGet = poolDictionary[_prefab].Dequeue();
        objectToGet.SetActive(true);
        objectToGet.transform.parent = null;

        return objectToGet;
    }

    //之所以要创建那个PooledObject就是因为这个函数
    //因为当调用return函数时，传入的并不是prefab，而是生成的GameObjet，而这些GameObject都不相同
    //所以字典就会检索不到，字典的key都是相应的prefab，所以需要用PooledObject这个类来存储prefab，这样可以方便获取
    public void ReturnToPool(GameObject _object)
    {
        GameObject originalPrefab = _object.GetComponent<PooledObject>().originalPrefab;

        _object.SetActive(false);
        _object.transform.parent = transform;

        poolDictionary[originalPrefab].Enqueue(_object);
    }

    //返回到对象池的协程函数
    public void ReturnToPoolDelay(float _delay, GameObject objectToReturn)
        => StartCoroutine(DelayReturn(_delay, objectToReturn));

    private IEnumerator DelayReturn(float delay, GameObject objectToRrturn)
    {
        yield return new WaitForSeconds(delay);

        ReturnToPool(objectToRrturn);
    }

    private void InitializeNewPool(GameObject _prefab)
    {
        poolDictionary[_prefab] = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(_prefab);
        }
    }

    private void CreateNewObject(GameObject _prefab)
    {
        //创建的时候，设置当前类为父节点
        GameObject newObject = Instantiate(_prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = _prefab;
        newObject.SetActive(false);

        poolDictionary[_prefab].Enqueue(newObject);
    }
}
