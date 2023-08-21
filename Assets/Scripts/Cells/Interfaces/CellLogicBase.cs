using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class CellLogicBase : MonoBehaviour, ICellLogic
{
    private static readonly float POS_DELTA = 0.01f;

    public bool Interactive { get; private set; } = true;

    public Action<ICellLogic> OnClick { get; set; }
    public ICellSlot LeftCell { get; private set; } = null;
    public ICellSlot UpCell { get; private set; } = null;
    public ICellSlot RightCell { get; private set; } = null;
    public ICellSlot DownCell { get; private set; } = null;

    private ICellSlot _curSlot = null;
    private BoxCollider2D _collider = null;

    public void BuildCellRefs()
    {
        _curSlot = GetComponent<ICellSlot>();
        StartCoroutine(BuildRefsPlay());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Interactive)
            OnClick?.Invoke(this);
    }

    public void Dispose()
    {
        LeftCell = null;
        UpCell = null;
        RightCell = null;
        DownCell = null;
    }

    private IEnumerator BuildRefsPlay()
    {
        gameObject.name = $"Cell[{_curSlot.Info.Position.x},{_curSlot.Info.Position.y}]";

        if(_collider == null)
            _collider = gameObject.GetComponent<BoxCollider2D>();

        _collider.size = Vector2.one * 0.5f;
        yield return new WaitForFixedUpdate();

        _collider.size = Vector2.one * 1.5f;
        yield return new WaitForSecondsRealtime(0.05f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        ICellSlot cell = col.GetComponent<ICellSlot>();
        if (cell != null && col.gameObject != this.gameObject)
        {
            SetRefToSlot(cell);
        }
    }

    private void SetRefToSlot(ICellSlot cell)
    {
        float deltaX = transform.position.x - cell.transform.position.x;
        float deltaY = transform.position.y - cell.transform.position.y;

        if (Mathf.Abs(deltaY) < POS_DELTA)
        {
            if (deltaX > POS_DELTA)
            {
                LeftCell = cell;
            }
            else
            {
                RightCell = cell;
            }
        }
        else if (Mathf.Abs(deltaX) < POS_DELTA)
        {
            if (deltaY > POS_DELTA)
            {
                DownCell = cell;
            }
            else
            {
                UpCell = cell;
            }
        }
    }
}