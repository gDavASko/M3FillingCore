using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICellLogic: IDisposable, IPointerClickHandler
{
    System.Action<ICellLogic> OnClick { get; set; }
    bool Interactive { get; }

    ICellLogic LeftCell { get; }
    ICellLogic UpCell { get; }
    ICellLogic RightCell { get; }
    ICellLogic DownCell { get; }

    Transform transform { get; }
    ICellSlot Slot { get; }

    void BuildCellRefs();
}