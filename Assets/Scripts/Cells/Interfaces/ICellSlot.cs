
using UnityEngine;

public interface ICellSlot: IPoolable<ICellSlot>
{
    bool CanPutChip { get; }
    System.Action<ICellSlot> OnCellFree { get; set; }

    void SetChip(IChip chip);
    void SetCover(ICover cover);
    void Init(IChip chip, ICover cover, IGenerator generator, Vector3 position, CellConfig info);
}