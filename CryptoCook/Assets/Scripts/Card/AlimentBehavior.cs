using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class AlimentBehavior : CardBehavior
{
    public GameObject engagedEffect;
    public GameObject usedEffect;
    public TextMeshPro textName;
    public AlimentScriptable alimentLogic;

    public bool isInReserve = false;
    public bool isEngaged;
    public bool isUsedThisTurn;

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
            Debug.Log("rayscasted");
            int emplacementMask = LayerMask.GetMask("DropCard");
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, emplacementMask))
            {
                Debug.Log("touch a emplacement reserve");
                if (hit.transform.tag == "Reserve" && !isInReserve)
                {
                    Debug.Log("isReserve");
                    PlayerBehavior pl = hit.transform.parent.parent.GetComponent<PlayerBehavior>();
                    Debug.Log(pl.pseudo);
                    if (pl.statePlayer == PlayerBehavior.StatePlayer.PickupFoodPhase)
                    {
                        isInReserve = true;
                        pl.reserveCards.Add(this);
                        deckManager.CmdPickOnTable(this);
                        pl.statePlayer = PlayerBehavior.StatePlayer.PlayCardPhase;
                        // a toi mathis
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

    public void OnMouseUpAsButton()
    {
        if(player.statePlayer == PlayerBehavior.StatePlayer.PlayCardPhase)
        {
            if(isInReserve && !isUsedThisTurn)
            {
                if(!isEngaged)
                {
                    Engage();
                }
                else
                {
                    DisEngage();
                }
            }
        }
    }

    public void ResetForTurn()
    {
        isUsedThisTurn = false;
        if(engagedEffect != null)
            engagedEffect.SetActive(false);
        if (usedEffect != null)
            usedEffect.SetActive(false);
    }

    private void Engage()
    {
        isEngaged = true;
        if (engagedEffect != null)
            engagedEffect.SetActive(true);
        player.engagedAliment.Add(this);
    }

    private void DisEngage()
    {
        isEngaged = false;
        if (engagedEffect != null)
            engagedEffect.SetActive(false);
        player.engagedAliment.Remove(this);
    }

    public void UseToPlayCard()
    {
        isEngaged = false;
        isUsedThisTurn = true;
        if (usedEffect != null)
            usedEffect.SetActive(true);
        if (engagedEffect != null)
            engagedEffect.SetActive(false);
    }
}
