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

        yield return null;
    }

    public override IEnumerator OnNewCardPlayed(ChefCardBehaviour card, ChefCardBehaviour newCard)
    {

        yield return null;
    }

}
