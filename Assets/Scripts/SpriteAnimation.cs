using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Sprite Animation")]
public class SpriteAnimation : ScriptableObject
{
    public string title;
    public List<Sprite> animationFrames;
    public float timePerFrameReset;
}
