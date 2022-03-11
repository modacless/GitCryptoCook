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

            card.player.PlaceAlimentInReserve(card.player.selectedAliment, false);
        }
    }
}
