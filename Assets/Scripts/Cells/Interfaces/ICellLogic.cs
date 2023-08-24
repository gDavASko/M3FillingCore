using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICellLogic: IDisposable, IPointerClickHandler
{
    bool Interactive { get; set; }

    ICellLogic LeftCell { get; }
    ICellLogic UpCell { get; }
    ICellLogic RightCell { get; }
    ICellLogic DownCell { get; }

    Transform transform { get; }
    ICellSlot Slot { get; }

    void Init(SlotEvents slotEvents);
    void BuildCellRefs();
    void ClickNear(ref List<ICellLogic> sameChips, string chipId);
    void AffectSlot();
    void ProcessNewChip(IChip chip);
    void TryPushDownChip();
    void CallNearChip();
}