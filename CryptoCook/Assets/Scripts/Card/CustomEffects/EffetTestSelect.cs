using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTestSelectEffect", menuName = "Cards/New Test Select Effect")]
public class EffetTestSelect : ScriptableEffect
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
        //code pour effet pour détruire une recette adverse

        card.player.selectedChefCard = null;
        card.player.StartSelectRecipeAlly();
        while (card.player.selectedChefCard == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffetPhase)
        {
            yield return new WaitForEndOfFrame();
        }

        if (card.player.selectedChefCard != null)
        {
            card.player.selectedChefCard.DestroyCard();
        }
        yield return null;
    }
}
