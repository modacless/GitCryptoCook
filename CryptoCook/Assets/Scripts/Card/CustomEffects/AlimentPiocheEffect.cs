using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlimentPiocheEffect", menuName = "Cards/New AlimentPiocheEffect")]
public class AlimentPiocheEffect : ScriptableEffect
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

        if (card.player.selectedAliment != null)
        {
            int duplicataNumber = 0;
            for (int i = 0; i < card.player.reserveCards.Count; i++)
            {
                if(card.player.reserveCards[i].alimentLogic == card.player.selectedAliment.alimentLogic)
                {
                    duplicataNumber++;
                }
            }

            for (int i = 0; i < duplicataNumber; i++)
            {
                card.player.PickupInDeckCuisine();
            }
        }
    }
}
