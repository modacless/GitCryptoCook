using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Card",menuName = "Cards/New card")]
public class ScriptableCard : ScriptableObject
{
    public enum CardType
    {
        Aliment,
        Recette,
        Effect
    }

    public CardType cardType;
     
    public MeshFilter meshFilterModel;
    public GameObject modelPrefab;

    public Material spriteBackground;

    public Material effectBackground;

    public Sprite effectIllu;

    public Material matRarity;

    public string cardName;
    public string TextDescription;
}

