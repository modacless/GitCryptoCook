using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static PlayerBehavior;

public class ChefCardBehaviour : CardBehavior
{
    public ChefCardScriptable cardLogic;

    public bool isOnBoard = false;

    public Repas repas;
    public int basePoint;
    public int variablePoint;

    [HideInInspector]
    [SyncVar] public int emplacementHand = -1;

    public void InitializeCard(ChefCardScriptable card)
    {
        cardLogic = card;
        basePoint = cardLogic.pointEarn;
        textName.text = cardLogic.cardName;
    }

    public override void OnMouseDrag()
    {
        if (hasAuthority && !isOnBoard)
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
                if (hit.transform.tag == "Board" && !isOnBoard)
                {
                    PlayerBehavior pl = hit.transform.parent.parent.GetComponent<PlayerBehavior>();
                    if (pl.statePlayer == PlayerBehavior.StatePlayer.PlayCardPhase)
                    {
                        isOnBoard = true;
                        pl.CmdDropCardOnBoard(this, pl.FindBoardPlaces(hit.transform.gameObject));
                    }
                    else
                    {
                        transform.position = basePosition;
                    }

                }
                else
                {
                    transform.position = basePosition;
                }
            }
            else
            {
                transform.position = basePosition;
            }
        }
    }

    public void RefreshEffect()
    {
        variablePoint = 0;
        StartCoroutine(cardLogic.effect.OnBoardChange(this));
    }
}
