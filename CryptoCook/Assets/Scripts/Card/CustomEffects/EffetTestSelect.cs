using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTestSelectEffect", menuName = "Cards/New Test Select Effect")]
public class EffetTestSelect : ScriptableEffect
{
    public override IEnumerator OnBoardChange(CardBehavior card)
    {
        yield return null;
    }

    public override IEnumerator OnUse(CardBehavior card)
    {
        //code pour effet pour détruire une recette adverse

        //card.playerBehaviour.selectedAdversaryRecipe = null;
        //card.playerBehaviour.StartAdversaryRecipeSelection();
        //while(card.playerBehaviour.selectedAdversaryRecipe == null && card.playerBehaviour.isSelecting)
        //{
        //      yield return new WaitForEndOfFrames();
        //}
        //
        //if(card.playerBehaviour.selectedAdversaryRecipe != null)
        //{
        //      card.playerBehaviour.selectedAdversaryRecipe.RemoveFromBoard();
        //}
        yield return null;
    }
}
