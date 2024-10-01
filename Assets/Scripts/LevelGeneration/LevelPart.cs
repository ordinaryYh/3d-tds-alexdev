using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("InterSection Check")]
    [SerializeField] private LayerMask intersectionLayer;
    [SerializeField] private Collider[] intersectionCheckColliders;
    [SerializeField] private Transform intersectionCheckParent;

    public bool IntersectionDetected()
    {
        Physics.SyncTransforms();

        foreach (var collider in intersectionCheckColliders)
        {
            Collider[] hitColliders =
Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, intersectionLayer);

            foreach (var hit in hitColliders)
            {
                InterSectionCheck interesectionCheck = hit.GetComponentInParent<InterSectionCheck>();

                if (interesectionCheck != null && intersectionCheckParent != interesectionCheck.transform)
                    return true;
            }

        }

        return false;

    }

    public void SnapAndAlignPartTo(SnapPoint _targetSnapPoint)
    {
        //相当是用另一个level的enter对准另一个level的exit
        SnapPoint enterPoint = GetEnterPoint();

        AlighTo(enterPoint, _targetSnapPoint);//首先要先进行旋转，然后才能连接
        SnapTo(enterPoint, _targetSnapPoint);
    }

    //还需要处理level的旋转，如果不处理，那么level结合会有明显问题
    //这个函数会处理旋转的部分
    //这个函数可能难以理解，需要画图自己进行理解
    //这个旋转算法是核心部分，也可以自行修改
    private void AlighTo(SnapPoint _ownSnapPoint, SnapPoint _targetSnapPoint)
    {
        //获取旋转的偏移值
        var rotationOffset =
            _ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;

        //然后将自身整体level的角度和targetPoint的角度设置为一致
        transform.rotation = _targetSnapPoint.transform.rotation;
        transform.Rotate(0, 180, 0);

        transform.Rotate(0, -rotationOffset, 0);
    }


    //进入到另一个snapPoint
    private void SnapTo(SnapPoint _ownSnapPoint, SnapPoint _targetSnapPoint)
    {
        //首先计算偏移值，自身的位置加上这个偏移值，就可以和下一个关卡的出口或者入口对应上
        var offset = transform.position - _ownSnapPoint.transform.position;

        var newPositon = _targetSnapPoint.transform.position + offset;
        transform.position = newPositon;
    }


    public SnapPoint GetEnterPoint() => GetSnapPointOfType(SnapPointType.Enter);
    public SnapPoint GetExitPoint() => GetSnapPointOfType(SnapPointType.Exit);

    private SnapPoint GetSnapPointOfType(SnapPointType _pointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();

        List<SnapPoint> filteredSnapPoints = new List<SnapPoint>();

        //将指定的type放在这个list里面
        foreach (SnapPoint snapPoint in snapPoints)
        {
            if (snapPoint.pointType == _pointType)
                filteredSnapPoints.Add(snapPoint);
        }

        //之后返回一个随机的snapPoint
        if (filteredSnapPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredSnapPoints.Count);
            return filteredSnapPoints[randomIndex];
        }

        return null;

    }
}
