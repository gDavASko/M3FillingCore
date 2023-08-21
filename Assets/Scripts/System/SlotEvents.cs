
public class SlotEvents
{
    public System.Action<ICellSlot> OnAddedSlot { get; set; }
    public System.Action OnClearSlots { get; set; }
    public System.Action OnSlotsViewCreated { get; set; }
}