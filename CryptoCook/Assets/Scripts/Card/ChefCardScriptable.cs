using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AlimentScriptable;

[SerializeField]
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
    public List<Cost> cost;

    [System.Serializable]
    public class Cost
    {
        public enum CostType
        {
            Gout,
            AlimentType,
            Specific,
        }

        public CostType costType;

        [DrawIf("costType", CostType.Gout)]
        public Gout goutCost;
        [DrawIf("costType", CostType.AlimentType)]
        public AlimentType alimentTypeCost;
        [DrawIf("costType", CostType.Specific)]
        public AlimentScriptable specificCost;
    }

    public ScriptableEffect effect;
    public int pointEarn;
}
