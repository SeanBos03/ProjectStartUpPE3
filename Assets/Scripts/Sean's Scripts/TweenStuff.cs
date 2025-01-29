using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TweenStuff : MonoBehaviour
{
    public Transform thePosition;
    public float theTime = 0.5f;
    public Vector3 originalPosition;
    public void StartMovingBack()
    {
        StopAllCoroutines();
        transform.DOMove(originalPosition, theTime);
    }
    public void StartMoving()
    {
        StopAllCoroutines();
        originalPosition = transform.position;
        transform.DOMove(thePosition.position, theTime);
    }

    public void MoveTo(Transform theTransform)
    {
        StopAllCoroutines();
        transform.DOMove(theTransform.position, theTime);
    }
}
