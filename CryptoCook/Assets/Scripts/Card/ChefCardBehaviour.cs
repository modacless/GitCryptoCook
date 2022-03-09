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

    public void InitializeCard(ChefCardScriptable card)
    {
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
                    PlayerBehavior pl = hit.transform.parent.parent.GetComponent<PlayerBehavior>();
                    if (pl.statePlayer == StatePlayer.PlayCardPhase)
                    {
                        isOnBoard = true;
                        pl.CmdDropCardOnBoard(this, pl.FindBoardPlaces(hit.transform.gameObject));
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
        GameObject camRef = deckManager.posCamP2;

        if(isClient)
        {
            camRef = deckManager.posCamP1;
        }

        transform.position = basePosition;
        float angle = Vector3.Angle(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(camRef.transform.position.x, 0, camRef.transform.position.z));
        if (transform.position.x <= camRef.transform.position.x)
        {
            angle = -angle;
        }
        transform.localRotation = Quaternion.Euler(50, angle, 0);
    }

    public void RefreshEffect()
    {
        variablePoint = 0;
        StartCoroutine(cardLogic.effect.OnBoardChange(this));
    }
}
