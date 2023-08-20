using UnityEngine;

public interface IChip: IPoolable<IChip>
{
    System.Action<IChip> OnClick { get; set; }
    bool Interactive { get; }

    void SetSlot(ICellSlot slot);
    void MoveAnimated(ICellSlot toSlot);
    void DestroyAnimated();
}