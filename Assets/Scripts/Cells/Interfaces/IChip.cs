using UnityEngine;

public interface IChip: IPoolable<IChip>
{
    void SetPoolReleaser(IPoolReleaser<IChip> poolReleaser);
    void MoveAnimated(ICellSlot toSlot);
    void DestroyAnimated();
}