using System.Collections;
using System.Collections.Generic;
using static ScriptableCard;
using UnityEngine;
using Mirror;


public class CardBehavior : NetworkBehaviour
{
    public ScriptableCard cardLogic;

    #region Drag parameters
    private float mZCoord;
    private Vector3 basePosition;

    private bool isInReserve = false;
    public CardArea area;

    #endregion

    void Start()
    {
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeCard(ScriptableCard card)
    {
        cardLogic = card;
        area = cardLogic.startArea;
    }

    public void OnMouseDown()
    {
        basePosition = transform.position;
    }

    public void OnMouseDrag()
    {
        if (hasAuthority)
        {
            Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mZCoord);
            Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(ScreenPosition);
            transform.position = newWorldPosition;
        }
    }

    public void OnMouseUp()
    {
        if (hasAuthority)
        {
            RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(transform.position,Vector3.down,out hit,Mathf.Infinity))
            {
                Debug.Log(hit.transform.name);
                if (hit.transform.tag == "Reserve" && !isInReserve && area == CardArea.OnGround)
                {
                    PlayerBehavior pl = hit.transform.parent.parent.GetComponent<PlayerBehavior>();
                    if (pl.yourTurn)
                    {
                        isInReserve = true;
                        pl.reserveCards.Add(this);
                        area = CardArea.InReserve;
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

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToViewportPoint(mousePoint);
    }

}
