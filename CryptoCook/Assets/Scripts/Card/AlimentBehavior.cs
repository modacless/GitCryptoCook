using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AlimentBehavior : CardBehavior
{
    public AlimentScriptable alimentLogic;

    public bool isInReserve = false;

    [HideInInspector]
    [SyncVar] public int emplacementFood = -1;
    public AlimentScriptable alimentData;

    public void InitializeCard(AlimentScriptable aliment)
    {
        alimentLogic = aliment;
        textName.text = alimentLogic.cardName;
    }

    public override void OnMouseDrag()
    {
        if (hasAuthority && !isInReserve)
        {
            Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mZCoord);
            Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(ScreenPosition);
            transform.position = newWorldPosition;
        }
    }

    public override void OnMouseUp()
    {
        if (hasAuthority)
        {
            RaycastHit hit;
            int emplacementMask = LayerMask.GetMask("DropCard");
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, emplacementMask))
            {
                if (hit.transform.tag == "Reserve" && !isInReserve)
                {
                    PlayerBehavior pl = hit.transform.parent.parent.GetComponent<PlayerBehavior>();
                    Debug.Log(pl.pseudo);
                    if (pl.statePlayer == PlayerBehavior.StatePlayer.PickupFoodPhase)
                    {
                        isInReserve = true;
                        pl.reserveCards.Add(this);
                        deckManager.CmdPickInReserve(this);
                        pl.statePlayer = PlayerBehavior.StatePlayer.PlayCardPhase;
                    }
                    else
                    {
                        transform.position = basePosition;
                    }
                }
            }
            else
            {
                transform.position = basePosition;
            }
        }
    }
}
