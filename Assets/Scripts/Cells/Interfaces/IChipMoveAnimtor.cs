using UnityEngine;

public interface IChipMoveAnimtor
{
   System.Action<ICellSlot> OnMoveComplete { get; set; }
   void MoveTo(ICellSlot to);
}