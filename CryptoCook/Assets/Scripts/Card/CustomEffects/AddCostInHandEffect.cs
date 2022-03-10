using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddCostInHandEffect", menuName = "Cards/New AddCostInHandEffect")]
public class AddCostInHandEffect : ScriptableEffect
{
    public string addedCost;

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
        PlayerBehavior otherPlayer = null;
        if(card.deckManager.playerClient.GetComponent<PlayerBehavior>() == card.player)
        {
            otherPlayer = card.deckManager.playerHost.GetComponent<PlayerBehavior>();
        }
        else
        {
            otherPlayer = card.deckManager.playerClient.GetComponent<PlayerBehavior>();
        }

        for (int i = 0; i < otherPlayer.handCards.Count; i++)
        {
            otherPlayer.handCards[i].CmdAddCost(addedCost);
            Debug.Log(otherPlayer.handCards[i].cardLogic.cardName + " got his cost increase with " + addedCost);
        }

        yield return null;
    }
}
