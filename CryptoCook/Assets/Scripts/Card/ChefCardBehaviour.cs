using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using static PlayerBehavior;

public class ChefCardBehaviour : CardBehavior
{
    public TextMeshPro textName;
    public ChefCardScriptable cardLogic;

    public bool isOnBoard = false;

    public Repas repas;
    public int basePoint;
    public int variablePoint;

    [HideInInspector]
    [SyncVar] public int emplacementHand = -1;

    public List<ChefCardScriptable.Cost> currentCost;

    public void InitializeCard(ChefCardScriptable card, PlayerBehavior _player)
    {
        player = _player;
        cardLogic = card;
        basePoint = cardLogic.pointEarn;
        textName.text = cardLogic.cardName;
        currentCost = new List<ChefCardScriptable.Cost>(cardLogic.cost);
    }

    public override void OnMouseDrag()
    {
        if (hasAuthority && !isOnBoard)
        {
            //Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mZCoord);
            //Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(ScreenPosition);
            //transform.position = newWorldPosition;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("DragPlane")))
            {
                transform.position = hit.point;
            }
        }
    }

    public override void OnMouseUp()
    {
        deckManager.dragPlane.SetActive(false);
        if (hasAuthority)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("DropCard")))
            {
                if (hit.transform.tag == "Board" && !isOnBoard)
                {
                    if (player.statePlayer == StatePlayer.PlayCardPhase)
                    {
                        if(player.CanPlayCard(this))
                        {
                            isOnBoard = true;
                            player.CmdDropCardOnBoard(this, player.FindBoardPlaces(hit.transform.gameObject));
                            player.UseEngagedAliment();
                        }
                        else
                        {
                            ResetToHand();
                        }
                    }
                    else
                    {
                        ResetToHand();
                    }

                }
                else
                {
                    ResetToHand();
                }
            }
            else
            {
                ResetToHand();
            }
        }
    }

    public void ResetToHand()
    {
        transform.position = basePosition;
        transform.localRotation = Quaternion.Euler(50f, 0, 0);
    }

    public void RefreshEffect()
    {
        variablePoint = 0;
        if(cardLogic.effect != null)
            StartCoroutine(cardLogic.effect.OnBoardChange(this));
    }
}
