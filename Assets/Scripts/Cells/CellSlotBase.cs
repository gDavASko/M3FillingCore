using System;
using UnityEngine;
using UnityEngine.Pool;

public class CellSlotBase : MonoBehaviour, ICellSlot
{
    [SerializeField] private Transform _mainBG = null;
    [SerializeField] private Transform _emptyBG = null;
    [SerializeField] private Transform _coverTrs = null;
    [SerializeField] private Transform _chipTrs = null;

    private IChip _chip = null;
    private ICover _cover = null;
    private IGenerator _generator = null;
    private IObjectPool<ICellSlot> _pool = null;
    private CellConfig _info = default;

    public string ID => "SlotBase";

    public CellConfig Info => _info;
    public bool CanPutChip => !_info.IsEmptySlot && _cover == null && _chip == null;

    public Action<ICellSlot> OnCellFree { get; set; }

    public Action OnReleased { get; set; }

    public void Init(IChip chip, ICover cover, IGenerator generator, Vector3 position, CellConfig info)
    {
        _info = info;
        transform.position = position;
        SetChip(chip);
        SetCover(cover);

        _generator = generator;

        _mainBG.gameObject.SetActive(!info.IsEmptySlot);

        if(_emptyBG != null)
            _emptyBG.gameObject.SetActive(info.IsEmptySlot);
    }

    public void SetChip(IChip chip)
    {
        OnChipLose();

        InitComponent<IChip>(chip, _chip, _chipTrs);

        if (chip != null)
        {
            chip.OnReleased += OnChipLose;
            chip.SetSlot(this);
        }
    }

    public void SetCover(ICover cover)
    {
        InitComponent<ICover>(cover, _cover, _coverTrs);
    }

    public void Release()
    {
        ReleaseComponents();

        OnReleased?.Invoke();

        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void DestroySelf()
    {
        ReleaseComponents();
        Destroy(this.gameObject);
    }

    public void SetPoolContainer(IObjectPool<ICellSlot> pool)
    {
        _pool = pool;
    }

    private void InitComponent<T>(T component, T slot, Transform container) where T : class, IPoolable<T>
    {
        slot = component;

        if (component != null)
        {
            component.transform.SetParent(container);
            component.transform.localPosition = Vector3.zero;
        }
    }

    private void OnChipLose()
    {
        if (_chip != null)
        {
            _chip.OnReleased -= OnChipLose;
            _chip = null;
        }
    }

    private void ReleaseComponents()
    {
        if(_chip != null)
            _chip.Release();

        if(_cover != null)
            _cover.Release();

        if (_generator != null)
            _generator.Release();
    }
}