using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgChg : MonoBehaviour
{
    Sprite sprtieOrginal;
   public Sprite spriteChange;

    SpriteRenderer theSR;

    public void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
        sprtieOrginal = theSR.sprite;
    }

    public void ChangeOrginal()
    {
        theSR.sprite = sprtieOrginal;
    }

    public void ChangeSprite()
    {
        theSR.sprite = spriteChange;
    }
}
