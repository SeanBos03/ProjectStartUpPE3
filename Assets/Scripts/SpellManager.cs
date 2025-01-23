using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public Transform Discard_Pile;
    public List<AnimationCurve> CardDiscard;
    // Start is called before the first frame update
    void Start()
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
    }

    IEnumerator MoveUsingThreeCurves(Vector3 origin, Vector3 DiscardPile, float duration, AnimationCurve CardDiscardX, AnimationCurve CardDiscardY, AnimationCurve CardDiscardZ)
    {
        float timePassed = 0f;
        while(timePassed <= duration)
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


    // Update is called once per frame
    void Update()
    { 
        StartCoroutine(MoveUsingCurve(this.transform.position, Discard_Pile.position, 0.5f, CardDiscard[0]));
    }
}
