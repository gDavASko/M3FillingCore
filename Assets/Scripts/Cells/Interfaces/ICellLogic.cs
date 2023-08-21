using System;
using UnityEngine.EventSystems;

public interface ICellLogic: IDisposable, IPointerClickHandler
{
    System.Action<ICellLogic> OnClick { get; set; }
    bool Interactive { get; }

    ICellSlot LeftCell { get; }
    ICellSlot UpCell { get; }
    ICellSlot RightCell { get; }
    ICellSlot DownCell { get; }

    void BuildCellRefs();
}