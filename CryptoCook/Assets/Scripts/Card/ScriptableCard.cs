using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card",menuName = "Cards/New card")]
public class ScriptableCard : ScriptableObject
{
    public MeshFilter meshFilterModel;
    public MeshRenderer meshRendererModel;

    public string cardName;
    public string pseudoName;

}
