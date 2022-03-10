using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AddPointToSameRepas", menuName = "Cards/AddPointToSameRepas")]

public class AddPointsToSameRepas : ScriptableEffect
{
    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {
        for (int i = 0; i < card.player.boardRepas.Count; i++)//Pour tout les repas
        {
            if (card.player.boardRepas[i].allRecipes.Count == card.repas.allRecipes.Count)
            {

                bool[] validatedRecipe = new bool[card.repas.allRecipes.Count];

                for (int j = 0; j < card.repas.allRecipes.Count; j++)//Pour toute les recettes du repas de base
                {
                    for (int y = 0; y < card.player.boardRepas[i].allRecipes.Count; y++)//Pour toute les recettes du repas en cours d'analyse.
                    {
                        if (card.repas.allRecipes[j].cardLogic == card.player.boardRepas[i].allRecipes[y].cardLogic && !validatedRecipe[y])
                        {
                            validatedRecipe[y] = true;
                            break;
                        }
                    }
                }

                bool isAllTheSame = true;
                for (int t = 0; t < validatedRecipe.Length; t++)
                {
                    if(!validatedRecipe[t])
                    {
                        isAllTheSame = false;
                    }
                }

                if(isAllTheSame && card.repas != card.player.boardRepas[i])
                {
                    card.player.boardRepas[i].variablePoint += 2;
                }
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
