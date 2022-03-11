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
        //code pour effet pour d�truire une recette adverse
        if (card.player.hasAuthority)
        {
            card.player.selectedChefCard = null;
            card.player.StartSelectRecipeEnemy();
            while (card.player.selectedChefCard == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffectPhase)
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
}
