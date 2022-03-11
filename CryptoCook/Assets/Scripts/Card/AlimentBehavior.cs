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
    public MeshRenderer foodSprite;

    public GameObject engagedEffect;
    public GameObject usedEffect;
    public AlimentScriptable alimentLogic;
    public float scaleReserveMultiplier;

    [SyncVar]public bool isInReserve = false;
    public bool isEngaged;
    public bool isUsedThisTurn;

    [HideInInspector]
    [SyncVar] public int emplacementFood = -1;

    public void InitializeCard(AlimentScriptable aliment)
    {
        alimentLogic = aliment;
        textName.text = alimentLogic.cardName;
        textType.text = alimentLogic.alimentType.ToString();
        textGout.text = alimentLogic.gout.ToString();
        foodSprite.GetComponent<MeshRenderer>().material = alimentLogic.sprite;

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
                    PlayerBehavior pl = deckManager.authorityPlayer;
                    if (pl.statePlayer == PlayerBehavior.StatePlayer.PickupFoodPhase)
                    {
                        player = pl;
                        targetScale = baseScale * (isInReserve ? scaleReserveMultiplier : 1);
                        player.PlaceAlimentInReserve(this, true);
                        SetCurrentPosAsBase();

                        AudioManager.AMInstance.PlaySFX(AudioManager.AMInstance.dropCardSFX, 1.2f);
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
                    else
                    {
                        Debug.Log("aliment is used or not in reesrve");
                    }
                }
                else
                {
                    Debug.Log("player is not in right phase");
                }
            }
            else
            {
                Debug.Log("player is null");
            }
        }
    }

    public override void OnMouseExit()
    {
        targetScale = baseScale * (isInReserve ? scaleReserveMultiplier : 1);
    }

    public override void OnMouseEnter()
    {
        targetScale = baseScale * hoverScaleMultiplier * (isInReserve ? scaleReserveMultiplier : 1);
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

    [Command(requiresAuthority = false)]
    public void CmdSetInReserve(bool isIn, PlayerBehavior newPlayerOwner)
    {
        isInReserve = isIn;
        netIdentity.AssignClientAuthority(newPlayerOwner.netIdentity.connectionToClient);
    }

    [Command(requiresAuthority = false)]
    public void CmdRemoveFromReserve()
    {
        RpcRemoveFromReserve();
        netIdentity.RemoveClientAuthority();
    }

    [ClientRpc]
    public void RpcRemoveFromReserve()
    {
        player.RemoveAliment(this);
    }


    [Command(requiresAuthority = false)]
    public void CmdMoveAliment(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}
