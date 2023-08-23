using UnityEngine;

public interface IChip: IPoolable<IChip>
{
    void SetSlot(ICellSlot slot);
    void MoveAnimated(ICellSlot toSlot, System.Action OnMoveComplete);
    void DestroyAnimated(System.Action OnDestroyComplete);
    void TweenScale();
}