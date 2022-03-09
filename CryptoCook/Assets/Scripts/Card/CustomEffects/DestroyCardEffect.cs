using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCardEffect : ScriptableEffect
{
    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {
        //code pour combo dans le repas


        for (int i = 0; i < card.repas.allRecipes.Count; i++)
        {
            if (card.repas.allRecipes[i].cardLogic.recipeType == ChefCardScriptable.RecipeType.Entree)
            {
                card.variablePoint += 2;
            }
        }

        yield return null;
    }

    public override IEnumerator OnUse(ChefCardBehaviour card)
    {
        yield return null;
    }

    public override IEnumerator OnNewCardPlayed(ChefCardBehaviour card, ChefCardBehaviour newCard)
    {
        yield return null;
    }
}
