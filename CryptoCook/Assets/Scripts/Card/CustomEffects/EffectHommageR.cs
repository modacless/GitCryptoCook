using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect Hommage Respectueux", menuName = "Cards/Effect Hommage Respectueux")]
public class EffectHommageR : ScriptableEffect
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
        card.player.StartSelectIngredientAlly();

        while (card.player.selectedAliment == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffectPhase)
        {
            yield return new WaitForEndOfFrame();
        }
        if(card.player.selectedAliment != null)
        {
            card.player.CmdDestroyFood(card.player.selectedAliment);
        }
        card.player.selectedAliment = null;

        card.player.selectedChefCard = null;
        card.player.StartSelectRecipeEnemy();

        while (card.player.selectedChefCard == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffectPhase)
        {
            yield return new WaitForEndOfFrame();
        }
        card.player.CmdStealRecipe(card.player, card.player.selectedChefCard);
        card.player.selectedChefCard = null;
        yield return null;
    }


}
