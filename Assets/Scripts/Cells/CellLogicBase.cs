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

    public bool IsBusy { get; }
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
        Slot.Affect(() =>
        {
            if(Slot.Generator != null)
                _slotEvents.OnGeneratorEmpty?.Invoke(this);

            AffectNearSlots();

            if(UpCell != null)
                UpCell.TryPushDownChip(true);
        });
    }

    public void ProcessNewChip(IChip chip)
    {
        Slot.SetChip(chip, false, () =>
        {
            TryPushDownChip(false);
        });
    }

    private void AffectNearSlots()
    {
        if(UpCell != null)
            UpCell.Slot.AffectAsNear(() => UpCell.TryPushDownChip(false));

        if(RightCell != null)
            RightCell.Slot.AffectAsNear(() => RightCell.TryPushDownChip(false));

        if(DownCell != null)
            DownCell.Slot.AffectAsNear(() => DownCell.TryPushDownChip(false));

        if(LeftCell != null)
            LeftCell.Slot.AffectAsNear(() => LeftCell.TryPushDownChip(false));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactive || Slot.Chip == null)
            return;

        var cellsNear = new List<ICellLogic>();
        ClickNear(ref cellsNear, Slot.Chip.ID);

        if (cellsNear.Count >= MATCH_COUNT)
        {
            foreach (ICellLogic logic in cellsNear)
            {
                logic.AffectSlot();
            }
        }
        else
        {
            Slot.Chip.TweenScale();
        }
    }

    public void ClickNear(ref List<ICellLogic> sameChips, string chipId)
    {
        if (Slot.Chip != null && Slot.Cover == null
                                     && Slot.Chip.ID == chipId && !sameChips.Contains(this))
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

    public void TryPushDownChip(bool delayed)
    {
        if (Slot.Cover != null || Slot.Info.IsEmptySlot || Slot.Chip == null)
            return;

        if (delayed)
        {
            StartCoroutine(PushDownDelayPlay());
        }
        else
        {
            TryMoveChipToSlot(DownCell);
        }
    }

    private IEnumerator PushDownDelayPlay()
    {
        yield return new WaitForFixedUpdate();
        TryMoveChipToSlot(DownCell);
    }

    private void TryMoveChipToSlot(ICellLogic slot)
    {
        if (slot != null && slot.Slot.CanPutChip)
        {
            var chip = Slot.Chip;
            Slot.ForgetChip();

            slot.Slot.SetChip(chip, true, () =>
            {
                if(Slot.Generator != null)
                    _slotEvents.OnGeneratorEmpty?.Invoke(this);

                if(UpCell != null)
                    UpCell.TryPushDownChip(true);

                slot.TryPushDownChip(true);
            });
        }
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
        if(Slot.Chip != null)
            TryPushDownChip(true);
    }

    private void SubscribeSlot()
    {
        _slotEvents.OnAddedChip += OnChipAdded;
    }

    private void UnsubscribeSlot()
    {
        if (_slotEvents == null)
            return;

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

        if (_curSlot != null)
        {
            _curSlot.Release();
            _curSlot = null;
        }

        UnsubscribeSlot();
    }
}