
using UnityEngine;

public interface ICellSlot: IPoolable<ICellSlot>
{
    IChip Chip { get; }
    ICover Cover { get; }
    IGenerator Generator { get; }
    CellConfig Info { get; }
    bool CanPutChip { get; }
    System.Action<ICellSlot> OnCellFree { get; set; }

    void SetChip(IChip chip, bool withAnimation, System.Action OnSetComplete);
    void ForgetChip();
    void SetCover(ICover cover);
    void Init(IChip chip, ICover cover, IGenerator generator, Vector3 position, CellConfig info);
    void Affect(System.Action OnAffectComplete);
    void AffectAsNear(System.Action OnAffectComplete);
}