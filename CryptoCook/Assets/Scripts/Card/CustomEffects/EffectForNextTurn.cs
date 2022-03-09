using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectForNextTurn", menuName = "Cards/EffectForNextTurn")]

public class EffectForNextTurn : ScriptableEffect
{
    public override IEnumerator OnBoardChange(ChefCardBehaviour card)
    {

        yield return null;
    }

    public override IEnumerator OnUse(ChefCardBehaviour card)
    {

        yield return null;
    }

    public override IEnumerator OnNewCardPlayed(ChefCardBehaviour card, ChefCardBehaviour newCard)
    {
        if(newCard)

        yield return null;
    }
}
