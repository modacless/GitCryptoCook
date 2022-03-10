using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AddPointToSameRepas", menuName = "Cards/AddPointToSameRepas")]

public class AddPointsToSameRepas : ScriptableEffect
{
    public PlayerBehavior player;


    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {
        for (int i = 0; i > player.boardRepas.Count; i++)//Pour tout les repas
        {
            if(player.boardRepas[i].allRecipes.Count == card.repas.allRecipes.Count)
            {
                for (int y = 0; y > player.boardRepas[i].allRecipes.Count; y++)//Pour toute les recettes du repas en cours d'analyse.
                {
                    bool isValidate = false;

                    for (int z = 0; z > card.repas.allRecipes.Count; z++)//Pour toute les recette du repas d'origine
                    {
                        if (player.boardRepas[i].allRecipes[y].name == card.repas.allRecipes[z].name)
                        {
                            isValidate = true;
                        }
                    }

                    if(!isValidate)
                    {
                        break;
                    }
                    else if(isValidate)
                    {
                        player.boardRepas[i].basePoint += 2;
                    }

                }
            }

        }





            for (int i = 0; i > player.boardRepas.Count; i++)
        {
            if (player.boardRepas[i].allRecipes.Count == 2)
            {
                player.boardRepas[i].basePoint++;
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
