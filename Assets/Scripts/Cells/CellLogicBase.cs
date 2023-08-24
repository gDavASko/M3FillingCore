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
    private Coroutine generatorCheck = null;
    private ICellSlot _curSlot = null;
    private BoxCollider2D _collider = null;

    private bool hasUp = false;
    private bool hasLeft = false;
    private bool hasDown = false;
    private bool hasRight = false;

    public bool Interactive { get; set; } = false;

    public ICellLogic LeftCell { get; private set; } = null;
    public ICellLogic UpCell { get; private set; } = null;
    public ICellLogic RightCell { get; private set; } = null;
    public ICellLogic DownCell { get; private set; } = null;

    public ICellSlot Slot => _curSlot;


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

        hasUp = false;
        hasLeft = false;
        hasDown = false;
        hasRight = false;

        StartCoroutine(BuildRefsPlay());
    }

    public void ProcessNewChip(IChip chip)
    {
        _slotEvents.OnChipMoved?.Invoke();
        Slot.SetChip(chip, false, () => { TryPushDownChip(); });

        if (generatorCheck != null)
            StopCoroutine(generatorCheck);
        generatorCheck = StartCoroutine(CheckGeneration());
    }

    public void AffectSlot()
    {
        Slot.Affect(() =>
        {
            if (Slot.Generator != null)
                _slotEvents.OnGeneratorEmpty?.Invoke(this);

            AffectNearSlots();

            if (UpCell != null)
                UpCell.TryPushDownChip();
        });
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
                _slotEvents.OnSlotAffected?.Invoke();
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

            if (hasUp)
                UpCell.ClickNear(ref sameChips, chipId);

            if (hasRight)
                RightCell.ClickNear(ref sameChips, chipId);

            if (hasDown)
                DownCell.ClickNear(ref sameChips, chipId);

            if (hasLeft)
                LeftCell.ClickNear(ref sameChips, chipId);
        }
    }

    public void TryPushDownChip()
    {
        if (Slot.Cover != null || Slot.Info.IsEmptySlot || Slot.Chip == null)
            return;

        StartCoroutine(PushDownDelayPlay());
    }

    public void CallNearChip()
    {
        if (hasUp)
        {
            if (UpCell.Slot.Chip != null)
            {
                UpCell.TryPushDownChip();
            }
            else
            {
                UpCell.CallNearChip();
            }
        }
        else if (hasLeft || hasRight)
        {
            if (hasLeft && LeftCell.Slot.Chip != null)
            {
                LeftCell.TryPushDownChip();
            }
            else if (hasRight)
            {
                RightCell.TryPushDownChip();
            }
        }
    }

    private IEnumerator CheckGeneration()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        if (Slot.Chip != null)
        {
            TryPushDownChip();
        }
        else if (Slot.Generator != null)
        {
            _slotEvents.OnGeneratorEmpty(this);
        }
    }

    private void AffectNearSlots()
    {
        if (hasUp)
            UpCell.Slot.AffectAsNear(() => UpCell.TryPushDownChip());

        if (hasRight)
            RightCell.Slot.AffectAsNear(() => RightCell.TryPushDownChip());

        if (hasDown)
            DownCell.Slot.AffectAsNear(() => DownCell.TryPushDownChip());

        if (hasLeft)
            LeftCell.Slot.AffectAsNear(() => LeftCell.TryPushDownChip());
    }

    private IEnumerator PushDownDelayPlay()
    {
        yield return new WaitForFixedUpdate();
        TryMoveChipToSlot();
    }

    private void TryMoveChipToSlot()
    {
        if (hasDown)
        {
            if (DownCell.Slot.CanPutChip)
            {
                var chip = Slot.Chip;
                Slot.ForgetChip();

                _slotEvents.OnChipMoved?.Invoke();
                DownCell.Slot.SetChip(chip, true, () =>
                {
                    if (Slot.Generator != null)
                        _slotEvents.OnGeneratorEmpty?.Invoke(this);

                    if (hasUp)
                        UpCell.TryPushDownChip();

                    DownCell.TryPushDownChip();
                });
            }
            else
            {
                bool canMoveRight = DownCell.RightCell != null && (!hasRight || RightCell.Slot.Info.IsEmptySlot || RightCell.Slot.Cover != null);
                bool canMoveLeft = DownCell.LeftCell != null && (!hasLeft || LeftCell.Slot.Info.IsEmptySlot || LeftCell.Slot.Cover != null);

                if (canMoveRight || canMoveLeft)
                {
                    if (canMoveRight)
                    {
                        StartCoroutine(TryMoveChipToSlot(DownCell.RightCell));
                    }
                    else
                    {
                        StartCoroutine(TryMoveChipToSlot(DownCell.LeftCell));
                    }
                }
                else if (Slot.Generator != null)
                {
                    if (hasLeft && LeftCell.Slot.Generator == null)
                    {
                        StartCoroutine(TryMoveChipToSlot(LeftCell));
                    }
                    else if (hasRight && RightCell.Slot.Generator == null)
                    {
                        StartCoroutine(TryMoveChipToSlot(RightCell));
                    }
                }
            }
        }
    }

    private IEnumerator TryMoveChipToSlot(ICellLogic toSlot)
    {
        yield return new WaitForFixedUpdate();

        if (toSlot != null && toSlot.Slot.CanPutChip)
        {
            var chip = Slot.Chip;
            Slot.ForgetChip();

            _slotEvents.OnChipMoved?.Invoke();
            toSlot.Slot.SetChip(chip, true, () =>
            {
                if (Slot.Generator != null)
                    _slotEvents.OnGeneratorEmpty?.Invoke(this);

                if (hasUp)
                    UpCell.TryPushDownChip();

                toSlot.TryPushDownChip();
            });
        }
    }

    private IEnumerator BuildRefsPlay()
    {
        gameObject.name = $"Cell[{_curSlot.Info.Position.x},{_curSlot.Info.Position.y}]";

        if (_collider == null)
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
                hasLeft = true;
            }
            else
            {
                RightCell = cell;
                hasRight = true;
            }
        }
        else if (Mathf.Abs(deltaX) < POS_DELTA)
        {
            if (deltaY > POS_DELTA)
            {
                DownCell = cell;
                hasDown = true;
            }
            else
            {
                UpCell = cell;
                hasUp = true;
            }
        }
    }

    private void OnChipAdded()
    {
        if (Slot.Chip != null)
            TryPushDownChip();
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