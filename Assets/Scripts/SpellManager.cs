using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public Transform Discard_Pile;
    public List<AnimationCurve> CardDiscard;
    public float theTime = 0.5f;
    public Vector3 originalPosition;
    public bool isMoving;
    public bool isMovingBack;

    private void Start()
    {
    }
    IEnumerator MoveUsingCurve(Vector3 origin, Vector3 Discard_Pile, float duration, AnimationCurve CardDiscard)
    {
        float timePassed = 0f;
        while (timePassed <= duration)
        {
            timePassed = timePassed + Time.deltaTime;
            float percent = Mathf.Clamp01(timePassed / duration);
            float curvePercent = CardDiscard.Evaluate(percent);
            transform.position = Vector3.LerpUnclamped(origin, Discard_Pile, curvePercent);

            yield return null;
        }

        //if (isMoving)
        //{
        //    isMoving = false;
        //}

        //if (isMovingBack)
        //{
        //    isMovingBack = false;
        //}
    }

    IEnumerator MoveUsingThreeCurves(Vector3 origin, Vector3 DiscardPile, float duration, AnimationCurve CardDiscardX, AnimationCurve CardDiscardY, AnimationCurve CardDiscardZ)
    {
        float timePassed = 0f;
        while (timePassed <= duration)
        {
            timePassed = timePassed + Time.deltaTime;
            float percent = Mathf.Clamp01(timePassed / duration);
            float curvePercentX = CardDiscardX.Evaluate(percent);
            float curvePercentY = CardDiscardY.Evaluate(percent);
            float curvePercentZ = CardDiscardZ.Evaluate(percent);
            transform.position = new Vector3(Mathf.LerpUnclamped(origin.x, DiscardPile.x, curvePercentX), Mathf.LerpUnclamped(origin.y, DiscardPile.y, curvePercentY), Mathf.LerpUnclamped(origin.x, DiscardPile.x, curvePercentX));

            yield return null;
        }
    }

    public void StartMovingBack()
    {
        StopAllCoroutines();
        //isMovingBack = true;
        //isMoving = false;
        //StartCoroutine(MoveUsingCurve(transform.position, orginalPosition, theTime, CardDiscard[1]));
      //  Debug.Log($"{name} {originalPosition}");
        transform.DOMove(originalPosition, theTime);
    }
    public void StartMoving()
    {
        StopAllCoroutines();
        originalPosition = transform.position;
      //  Debug.Log($"{name} {originalPosition}");
        //isMoving = true;
        //isMovingBack = false;
        transform.DOMove(Discard_Pile.position, theTime);
        //StartCoroutine(MoveUsingCurve(transform.position, Discard_Pile.position, theTime, CardDiscard[0]));
    }
    void Update()
    {
        if (isMoving)
        {
            //StartCoroutine(MoveUsingCurve(transform.position, Discard_Pile.position, theTime, CardDiscard[0]));
        }

        if (isMovingBack)
        {
            //StartCoroutine(MoveUsingCurve(transform.position, orginalPosition, theTime, CardDiscard[1]));
        }
    }
}
