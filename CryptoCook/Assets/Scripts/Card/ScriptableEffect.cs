using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableEffect : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public abstract IEnumerator OnUse(ChefCardBehaviour card);

    public abstract IEnumerator OnBoardChange(ChefCardBehaviour card);

    public abstract IEnumerator OnNewCardPlayed(ChefCardBehaviour card, ChefCardBehaviour newCard);
}
