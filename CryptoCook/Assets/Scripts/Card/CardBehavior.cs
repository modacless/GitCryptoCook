using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using static ScriptableCard;
using static PlayerBehavior;
using UnityEngine;
using Mirror;


public abstract class CardBehavior : NetworkBehaviour
{
    #region références
    public DeckManager deckManager;
    #endregion

    #region Drag parameters
    protected float mZCoord;
    protected Vector3 basePosition;
    #endregion

    public PlayerBehavior player;

    private UnityEvent OnUse;

    void Start()
    {
        deckManager = GameObject.Find("GameManager").GetComponent<DeckManager>();
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;

    }

    public void OnMouseDown()
    {
        basePosition = transform.position;
    }

    public abstract void OnMouseDrag();

    public abstract void OnMouseUp();


    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToViewportPoint(mousePoint);
    }
    
}
