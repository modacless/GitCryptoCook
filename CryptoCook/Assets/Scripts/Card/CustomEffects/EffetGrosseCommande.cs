using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrosseCommande", menuName = "Cards/New GrosseCommande")]
public class EffetGrosseCommande : ScriptableEffect
{
    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {
        yield return null;
    }

    public override IEnumerator OnNewCardPlayed(ChefCardBehaviour card, ChefCardBehaviour newCard)
    {
        newCard.player.PickupInDeckCuisine();

        yield return null;
    }

    public override IEnumerator OnUse(ChefCardBehaviour card)
    {
        card.player.effectActiveThisTurn.Add(this);

        yield return null;
    }
}
