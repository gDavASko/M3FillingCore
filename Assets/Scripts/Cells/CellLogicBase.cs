using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellLogicBase : MonoBehaviour, ICellLogic
{
    private static readonly float POS_DELTA = 0.01f;
    private static readonly int MATCH_COUNT = 3;

    private SlotEvents _slotEvents = null;

    public bool Interactive { get; set; } = false;

    public Action<ICellLogic> OnClick { get; set; }
    public ICellLogic LeftCell { get; private set; } = null;
    public ICellLogic UpCell { get; private set; } = null;
    public ICellLogic RightCell { get; private set; } = null;
    public ICellLogic DownCell { get; private set; } = null;

    public ICellSlot Slot => _curSlot;

    private ICellSlot _curSlot = null;
    private BoxCollider2D _collider = null;

    public void Init(SlotEvents slotEvents)
    {
        if (_slotEvents != null)
        {
            UnsubscribeSlot();
        }

        _slotEvents = slotEvents;
        SubscribeSlot();

    }

    public void BuildCellRefs()
    {
        _curSlot = GetComponent<ICellSlot>();
        StartCoroutine(BuildRefsPlay());
    }

    public void AffectSlot()
    {
        Slot.Affect();
        AffectNearSlots();

        if(UpCell != null)
            UpCell.TryPushDownChip();

        _slotEvents.OnSlotAffected?.Invoke(this);
    }

    public void ProcessNewChip(IChip chip)
    {
        Debug.LogError($"Process new Chip for {name} for slot {Slot.transform.name}");
        Slot.SetChip(chip, false);
        TryPushDownChip();
    }

    private void AffectNearSlots()
    {
        if(UpCell != null)
            UpCell.Slot.AffectAsNear();

        if(RightCell != null)
            RightCell.Slot.AffectAsNear();

        if(DownCell != null)
            DownCell.Slot.AffectAsNear();

        if(LeftCell != null)
            LeftCell.Slot.AffectAsNear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactive || Slot.CurrentChip == null)
            return;

        OnClick?.Invoke(this);
        var cellsNear = new List<ICellLogic>();
        ClickNear(ref cellsNear, Slot.CurrentChip.ID);

        if (cellsNear.Count >= MATCH_COUNT)
        {
            foreach (ICellLogic logic in cellsNear)
            {
                logic.AffectSlot();
            }
        }
        else
        {
            Slot.CurrentChip.TweenScale();
        }
    }

    public void ClickNear(ref List<ICellLogic> sameChips, string chipId)
    {
        if (Slot.CurrentChip != null && Slot.CurrentCover == null
                                     && Slot.CurrentChip.ID == chipId && !sameChips.Contains(this))
        {
            sameChips.Add(this);

            if(UpCell != null)
                UpCell.ClickNear(ref sameChips, chipId);

            if(RightCell != null)
                RightCell.ClickNear(ref sameChips, chipId);

            if(DownCell != null)
                DownCell.ClickNear(ref sameChips, chipId);

            if(LeftCell != null)
                LeftCell.ClickNear(ref sameChips, chipId);
        }
    }

    public void TryPushDownChip()
    {
        //return;
        if (Slot.CurrentCover != null || Slot.Info.IsEmptySlot || Slot.CurrentChip == null)
            return;

        if (DownCell != null)
            TryMoveChipToSlot(DownCell);

        if (UpCell != null)
            TryMoveChipToSlot(UpCell);
    }

    private bool TryMoveChipToSlot(ICellLogic slot)
    {
        if (slot != null && slot.Slot.CanPutChip)
        {
            slot.Slot.SetChip(Slot.CurrentChip, true);
            _slotEvents.OnSlotAffected?.Invoke(this);
        }

        return false;
    }


    private IEnumerator BuildRefsPlay()
    {
        gameObject.name = $"Cell[{_curSlot.Info.Position.x},{_curSlot.Info.Position.y}]";

        if(_collider == null)
            _collider = gameObject.GetComponent<BoxCollider2D>();

        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        _collider.size = Vector2.one * 0.5f;
        yield return new WaitForFixedUpdate();

        _collider.size = Vector2.one * 1.5f;
        yield return new WaitForSecondsRealtime(0.1f);

        _collider.size = Vector2.one;

        Destroy(rb);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        ICellLogic cell = col.GetComponent<ICellLogic>();
        if (cell != null && col.gameObject != this.gameObject)
        {
            SetRefToSlot(cell);
        }
    }

    private void SetRefToSlot(ICellLogic cell)
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

    private void OnChipAdded()
    {
        if(Slot.CurrentChip != null)
            TryPushDownChip();
    }

    private void SubscribeSlot()
    {
        _slotEvents.OnAddedChip += OnChipAdded;
    }

    private void UnsubscribeSlot()
    {
        _slotEvents.OnAddedChip -= OnChipAdded;
        _slotEvents = null;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void Dispose()
    {
        LeftCell = null;
        UpCell = null;
        RightCell = null;
        DownCell = null;

        _curSlot.Release();
        _curSlot = null;

        UnsubscribeSlot();
    }
}