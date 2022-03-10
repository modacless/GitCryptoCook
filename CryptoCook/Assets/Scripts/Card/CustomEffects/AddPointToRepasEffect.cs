using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AddPointToRepasEffect", menuName = "Cards/AddPointToRepasEffect")]

public class AddPointToRepasEffect : ScriptableEffect
{
    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {

        yield return null;
    }

    public override IEnumerator OnUse(ChefCardBehaviour card)
    {
        for (int i = 0; i < card.player.boardRepas.Count; i++)
        {
            if(card.player.boardRepas[i].allRecipes.Count == 2)
            {
                card.player.boardRepas[i].basePoint++;
            }
        }

        yield return null;
    }

    public override IEnumerator OnNewCardPlayed(ChefCardBehaviour card, ChefCardBehaviour newCard)
    {

        yield return null;
    }

}
