using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableEffect : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public abstract IEnumerator OnUse(CardBehavior card);

    public abstract IEnumerator OnBoardChange(CardBehavior card);
}
