using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockAlimentWithPlacingRecipe", menuName = "Cards/New UnlockAlimentWithPlacingRecipe")]
public class UnlockAlimentWithPlacingRecipe : ScriptableEffect
{
    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {
        yield return null;
    }

    public override IEnumerator OnNewCardPlayed(ChefCardBehaviour card, ChefCardBehaviour newCard)
    {
        card.player.selectedAliment = null;
        card.player.StartSelectIngredientAlly();
        while(card.player.selectedAliment == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffectPhase)
        {
            yield return new WaitForEndOfFrame();
        }

        if(card.player.selectedAliment != null)
        {
            card.player.selectedAliment.ResetForTurn();
        }
    }

    public override IEnumerator OnUse(ChefCardBehaviour card)
    {

        yield return null;
    }
}
