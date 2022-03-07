using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTestEffect", menuName = "Cards/New Test Effect")]
public class EffetTest : ScriptableEffect
{
    public override IEnumerator OnBoardChange(CardBehavior card)
    {
        //code pour combo dans le repas


        //for(int i = 0; i < card.repas.allrecipe.length; i++)
        //{
        //  if(card.repas.allrecipe[i].type == RecipeType.Entree)
        //  {
        //      card.baseScore += 2;
        //  }
        //}

        yield return null;
    }

    public override IEnumerator OnUse(CardBehavior card)
    {
        yield return null;
    }
}
