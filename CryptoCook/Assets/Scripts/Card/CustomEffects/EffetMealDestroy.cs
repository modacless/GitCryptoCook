using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDestroyMeal", menuName = "Cards/EffectDestroyMeal")]
public class EffetMealDestroy : ScriptableEffect
{
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
        card.player.selectMeal = null;
        card.player.StartSelectMealAlly();
        while (card.player.selectMeal == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffetPhase)
        {
            yield return new WaitForEndOfFrame();
        }

        if (card.player.selectMeal != null)
        {
            if (card.player.selectMeal.allRecipes.Count > 0)
            {
                card.player.CmdDestroyMeal(card.player.selectMeal.allRecipes[0]);
            }
            
        }
        yield return null;
    }
}
