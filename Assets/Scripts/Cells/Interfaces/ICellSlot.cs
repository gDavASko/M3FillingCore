
using UnityEngine;

public interface ICellSlot: IPoolable<ICellSlot>
{
    IChip CurrentChip { get; }
    ICover CurrentCover { get; }
    IGenerator Generator { get; }
    CellConfig Info { get; }
    bool CanPutChip { get; }
    System.Action<ICellSlot> OnCellFree { get; set; }

    void SetChip(IChip chip, bool withAnimation);
    void SetCover(ICover cover);
    void Init(IChip chip, ICover cover, IGenerator generator, Vector3 position, CellConfig info);
    void Affect();
    void AffectAsNear();
}