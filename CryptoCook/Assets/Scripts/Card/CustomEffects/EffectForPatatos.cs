using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EffectForPatatos", menuName = "Cards/EffectForPatatos")]
public class EffectForPatatos : ScriptableEffect
{
    public int numberOfRecetteInRepas;
    public int numberOfCardToDraw;
    
    public bool isCulture;
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
        int numberOfPlatInRepas = 0;

                Debug.Log("je rentre dans la fonction");
                for (int i = 0; i < card.repas.allRecipes.Count; i++)
                {
                    if (card.repas.allRecipes[i].cardLogic.recipeCulture == ChefCardScriptable.Culture.Americain)
                    {
                        numberOfPlatInRepas += 1;
                        Debug.Log(("le nombre de recette dans ce plat est de" + numberOfPlatInRepas));
                    }
                }
                if ( numberOfPlatInRepas == numberOfRecetteInRepas)
                {
                    for (int i = 0; i < numberOfCardToDraw; i++)
                    {
                        card.player.PickupInDeckCuisine();
                    } 
                }
                    yield return null;
            
    }
        
    }
