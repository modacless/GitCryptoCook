using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectSushi", menuName = "Cards/EffectSushi")]
public class EffectSushi : ScriptableEffect
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
        card.player.selectedAliment = null;
        card.player.StartSelectTableIngredient(AlimentScriptable.Gout.Sucré);
        while(card.player.selectedAliment == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffectPhase)
        {
            yield return new WaitForEndOfFrame();
        }
        card.player.PlaceAlimentInReserve(card.player.selectedAliment,true);
        card.player.selectedAliment.UseToPlayCard();
        card.player.selectedAliment = null;

        yield return null;
    }
}
