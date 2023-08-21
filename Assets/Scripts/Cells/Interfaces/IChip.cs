using UnityEngine;

public interface IChip: IPoolable<IChip>
{
    void SetSlot(ICellSlot slot);
    void MoveAnimated(ICellSlot toSlot);
    void DestroyAnimated();
}