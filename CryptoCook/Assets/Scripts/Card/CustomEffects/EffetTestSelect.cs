using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTestSelectEffect", menuName = "Cards/New Test Select Effect")]
public class EffetTestSelect : ScriptableEffect
{
    public override IEnumerator OnBoardChange(CardBehavior card)
    {
        yield return null;
    }

    public override IEnumerator OnUse(CardBehavior card)
    {
        //code pour effet pour détruire une recette adverse

        card.player.selectedCard = null;
        card.player.StartSelectRecipeEnemy();
        while (card.player.selectedCard == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffetPhase)
        {
            yield return new WaitForEndOfFrame();
        }

        if (card.player.selectedCard != null)
        {
            //card.player.selectedCard.RemoveFromBoard();
        }
        yield return null;
    }
}
