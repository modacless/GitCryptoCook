using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddOneWithX", menuName = "Cards/AddOneWithX")]
public class AddOneWithX : ScriptableEffect
{
    public ChefCardScriptable recette1;
    public ChefCardScriptable recette2;
    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {
        bool isBonused = false;
        
        for (int i = 0; i < card.repas.allRecipes.Count; i++)
        {
            if (card.repas.allRecipes[i].cardLogic == recette1 || card.repas.allRecipes[i].cardLogic == recette2)
            {
                isBonused = true;
            }
        }

        if (isBonused)
        {
            card.variablePoint += 1;
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
