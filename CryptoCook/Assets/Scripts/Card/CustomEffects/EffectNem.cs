using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect Nem", menuName = "Cards/Effect Nem")]
public class EffectNem : ScriptableEffect
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
        if(card.repas.allRecipes.Count == 1)
        {
            card.player.selectedAliment = null;
            card.player.StartSelectTableIngredient(AlimentScriptable.Gout.Epicé);
            while (card.player.selectedAliment == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffectPhase)
            {
                yield return new WaitForEndOfFrame();
            }
            card.player.PlaceAlimentInReserve(card.player.selectedAliment, true);
        }
        yield return null;
    }

}
