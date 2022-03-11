using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect Mochi", menuName = "Cards/Effect Mochi")]
public class EffectMochi : ScriptableEffect
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
        bool hasEntree = false;
        if(card.repas != null)
        {
            for(int i =0; i < card.repas.allRecipes.Count; i++)
            {
                if(card.repas.allRecipes[i].cardLogic.recipeType == ChefCardScriptable.RecipeType.Entree)
                {
                    hasEntree = true;
                }
            }

            if (hasEntree)
            {
                card.player.StartSelectRecipeEnemy();
                while (card.player.selectedChefCard == null && card.player.statePlayer == PlayerBehavior.StatePlayer.EffectPhase)
                {
                    yield return new WaitForEndOfFrame();
                }
                card.player.CmdHandSend(card.player.selectedChefCard.player, card.player.selectedChefCard);

                yield return null;
            }
        }

        yield return null;
    }


}
