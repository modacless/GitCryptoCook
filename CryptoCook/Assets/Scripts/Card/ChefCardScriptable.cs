using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChefCard", menuName = "Cards/New Chef Card")]
public class ChefCardScriptable : ScriptableCard
{
    public enum RecipeType
    {
        Entree,
        Plat,
        Dessert
    }
    public enum Culture
    {
        Asiatique,
        Americain,
        Occidental,
        Oriental
    }

    public RecipeType recipeType;
    public Culture recipeCulture;

    public ScriptableEffect effect;
    public int pointEarn;
}
