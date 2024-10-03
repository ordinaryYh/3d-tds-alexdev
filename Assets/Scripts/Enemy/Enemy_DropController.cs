using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个类是用于enemy的mission
public class Enemy_DropController : MonoBehaviour
{
    [SerializeField] private GameObject missionObjectKey;

    public void GiveKey(GameObject newKey) => missionObjectKey = newKey;

    //死亡时会调用这个函数
    public void DropItems()
    {
        if (missionObjectKey != null)
            CreateItem(missionObjectKey);
    }


    private void CreateItem(GameObject _object)
    {
        GameObject newItem = Instantiate(_object, transform.position + Vector3.up, Quaternion.identity);
    }
}
