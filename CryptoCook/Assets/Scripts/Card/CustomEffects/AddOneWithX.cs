using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOneWithX : ScriptableEffect
{
    private bool canAddScore;
    public string aliment1;
    public string aliment2;
    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {
        for (int i = 0; i < card.repas.allRecipes.Count; i++)
        {
            if (card.repas.allRecipes[i].cardLogic.cardName == aliment1 || card.repas.allRecipes[i].cardLogic.cardName == aliment2)
            {
                if (canAddScore)
                {
                    card.variablePoint += 1;
                    canAddScore = false;
                }
            }
        }
        yield return null;
    }

    public override IEnumerator OnNewCardPlayed(ChefCardBehaviour card, ChefCardBehaviour newCard)
    {
        yield return null;
    }

    public override IEnumerator OnUse(ChefCardBehaviour card)
    {
      
        yield return null;
    }
}
