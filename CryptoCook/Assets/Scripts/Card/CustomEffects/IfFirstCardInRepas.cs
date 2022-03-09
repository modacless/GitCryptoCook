using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfFirstCardInRepas", menuName = "Cards/IfFirstCardInRepas")]
public class IfFirstCardInRepas : ScriptableEffect
{
    public int numberOfCardToDraw;
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
        if (card.repas.allRecipes.Count == 0)
        {
            for (int i = 0; i < numberOfCardToDraw; i++)
            {
                card.player.PickupInDeckCuisine();
            } 
        }
        yield return null;
    }
}
