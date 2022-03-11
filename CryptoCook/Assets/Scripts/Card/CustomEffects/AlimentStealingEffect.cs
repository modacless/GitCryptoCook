using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlimentStealingEffect", menuName = "Cards/New AlimentStealingEffect")]
public class AlimentStealingEffect : ScriptableEffect
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
        Debug.Log("Use ffect steal aliment");
        card.player.selectedAliment = null;
        card.player.StartSelectIngredientEnemy();

        while (card.player.selectedAliment == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffectPhase)
        {
            yield return new WaitForEndOfFrame();
        }

        if (card.player.selectedAliment != null)
        {
            card.player.selectedAliment.CmdRemoveFromReserve();

            while (card.player.selectedAliment.player != null)
            {
                yield return new WaitForEndOfFrame();
            }

            Debug.Log("ended");
            card.player.selectedAliment.player = card.player;
            Debug.Log("in effect : " + card.player);
            card.player.PlaceAlimentInReserve(card.player.selectedAliment, false);
        }
    }
}
