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
    public IChip CurrentChip => _chip;
    public ICover CurrentCover => _cover;
    public IGenerator Generator => _generator;
    public CellConfig Info => _info;
    public bool CanPutChip => !_info.IsEmptySlot && _cover == null && _chip == null;

    public Action<ICellSlot> OnCellFree { get; set; }

    public Action OnReleased { get; set; }

    public void Init(IChip chip, ICover cover, IGenerator generator, Vector3 position, CellConfig info)
    {
        _info = info;
        transform.position = position;
        SetChip(chip, false);
        SetCover(cover);

        _generator = generator;

        _mainBG.gameObject.SetActive(!info.IsEmptySlot);

        if(_emptyBG != null)
            _emptyBG.gameObject.SetActive(info.IsEmptySlot);
    }

    public void Affect()
    {
        if (_chip == null)
            return;

        _chip.DestroyAnimated();
        _chip = null;
    }

    public void AffectAsNear()
    {
        if(_cover != null)
            _cover.DealDamage(() => { _cover = null;});
    }

    public void SetChip(IChip chip, bool withAnimation)
    {
        OnChipLose();

        _chip = chip;

        if (_chip != null)
        {
            if (withAnimation)
            {
                _chip.MoveAnimated(this);
            }
            else
            {
                _chip.transform.SetParent(_chipTrs);
                _chip.transform.localPosition = Vector3.zero;
                _chip.transform.localScale = Vector3.one;
            }
        }

        if (chip != null)
        {
            chip.OnReleased += OnChipLose;
            chip.SetSlot(this);
        }
    }

    public void SetCover(ICover cover)
    {
        _cover = cover;

        if (_cover != null)
        {
            _cover.transform.SetParent(_coverTrs);
            _cover.transform.localPosition = Vector3.zero;
            _cover.transform.localScale = Vector3.one;
        }
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