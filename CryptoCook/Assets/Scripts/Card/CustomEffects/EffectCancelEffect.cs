using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectCancelEffect", menuName = "Cards/New EffectCancelEffect")]
public class EffectCancelEffect : ScriptableEffect
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
        card.player.selectedChefCard = null;
        card.player.StartSelectRecipeEnemy();

        while (card.player.selectedChefCard == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffectPhase)
        {
            yield return new WaitForEndOfFrame();
        }

        if(card.player.selectedChefCard != null)
        {
            card.player.selectedChefCard.CmdDisabelEffect();
            //Debug.Log(card.player.selectedChefCard.cardLogic.cardName + " effect active : " + card.player.selectedChefCard.isEffectActive);
            card.player.selectedChefCard.player.CmdRefreshBoard();
        }
    }
}
