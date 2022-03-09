using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.Chat;
using UnityEngine;

public class IfXCardInRepasDrawX : ScriptableEffect
{
    public int numberOfRecetteInRepas;
    public int numberOfCardToDraw;
    private int numberOfPlatInRepas = 0;
    public bool isCulture;
    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {
        yield return null;
    }

    public override IEnumerator OnNewCardPlayed(ChefCardBehaviour card, ChefCardBehaviour newCard)
    {
        yield return null;
    }

    public override IEnumerator OnUse(ChefCardBehaviour card)
    {
        if (!isCulture)
        {
            for (int i = 0; i < card.repas.allRecipes.Count; i++)
            {
                if (card.repas.allRecipes[i].cardLogic.recipeType == ChefCardScriptable.RecipeType.Plat)
                {
                    numberOfPlatInRepas += 1;
                }
            }
            if ( numberOfPlatInRepas >= numberOfRecetteInRepas)
            {
                for (int i = 0; i < numberOfCardToDraw; i++)
                {
                    card.player.PickupInDeckCuisine();
                } 
            }
            else if(isCulture)
            {
                for (int i = 0; i < card.repas.allRecipes.Count; i++)
                {
                    if (card.repas.allRecipes[i].cardLogic.recipeCulture == ChefCardScriptable.Culture.Americain)
                    {
                        numberOfPlatInRepas += 1;
                    }
                }
                if ( numberOfPlatInRepas == numberOfRecetteInRepas)
                {
                    for (int i = 0; i < numberOfCardToDraw; i++)
                    {
                        card.player.PickupInDeckCuisine();
                    } 
                }
            }
            
        }
        yield return null;
    }
}
