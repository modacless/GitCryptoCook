using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class AlimentBehavior : CardBehavior
{
    public TextMeshPro textName;
    public TextMeshPro textType;
    public TextMeshPro textGout;
    public SpriteRenderer foodSprite;

    public GameObject engagedEffect;
    public GameObject usedEffect;
    public AlimentScriptable alimentLogic;

    public bool isInReserve = false;
    public bool isEngaged;
    public bool isUsedThisTurn;

    [HideInInspector]
    [SyncVar] public int emplacementFood = -1;



    public void InitializeCard(AlimentScriptable aliment)
    {
        alimentLogic = aliment;
        textName.text = alimentLogic.cardName;
        textType.text = alimentLogic.cardType.ToString();
        textGout.text = alimentLogic.gout.ToString();

        if(alimentLogic.sprite != null)
        {
            foodSprite.sprite = alimentLogic.sprite;
        }

    }

    public override void OnMouseDrag()
    {
        if (hasAuthority && !isInReserve && !deckManager.authorityPlayer.cardIsZoom)
        {
            deckManager.authorityPlayer.isDraggingCard = true;
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
            deckManager.authorityPlayer.isDraggingCard = false;
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
                        player = pl;
                        pl.reserveCards.Add(this);
                        deckManager.CmdPickOnTable(this);
                        pl.statePlayer = PlayerBehavior.StatePlayer.PlayCardPhase;
                        SetCurrentPosAsBase();
                    }
                    else
                    {
                        ResetPos();
                    }
                }
                else
                {
                    ResetPos();
                }
            }
            else
            {
                ResetPos();
            }
        }
    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();

        if(hasAuthority)
        {
            if (player != null)
            {
                if (player.statePlayer == PlayerBehavior.StatePlayer.PlayCardPhase)
                {
                    if (isInReserve && !isUsedThisTurn)
                    {
                        if (!isEngaged)
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

    public bool UnUse()
    {
        bool wasUsed = isUsedThisTurn;
        if(isUsedThisTurn)
        {
            ResetForTurn();
        }

        return isUsedThisTurn;
    }
}
