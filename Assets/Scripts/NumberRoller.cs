using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NumberRoller : MonoBehaviour
{
    public List<Sprite> NumberSprites;

    private SpriteRenderer mSpriteRender = null;
    private void Start()
    {
        mSpriteRender = GetComponent<SpriteRenderer>();
    }

    public void SwitchNumbers() 
    {
        
    }
}
