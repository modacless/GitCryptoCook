using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlimentBehavior : CardBehavior
{
    // Start is called before the first frame update
    public enum AlimentType
    {
        Légume,
        Fruit,
        Viande,
        Féculent,
        Poisson
    }

    public AlimentType alimentType;

    public enum Gout
    {
        Salé,
        Sucré,
        Viande,
        Féculent,
        Poisson
    }

    public Gout gout;

    /*public void OnMouseUp()
    {
        if (hasAuthority)
        {
            RaycastHit hit;
            int emplacementMask = LayerMask.GetMask("DropCard");
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, emplacementMask))
            {
                if (hit.transform.tag == "Reserve" && !isInReserve && cardLogic.cardType == CardType.Aliment)
                {
                    PlayerBehavior pl = hit.transform.parent.parent.GetComponent<PlayerBehavior>();
                    Debug.Log(pl.pseudo);
                    if (pl.statePlayer == PlayerBehavior.StatePlayer.PickupFoodPhase)
                    {
                        isInReserve = true;
                        pl.reserveCards.Add(this);
                        deckManager.CmdPickInReserve((AlimentBehavior)this);
                        pl.statePlayer = PlayerBehavior.StatePlayer.PlayCardPhase;
                    }
                    else
                    {
                        transform.position = basePosition;
                    }
                }

            }

        }
        
    }*/

}
