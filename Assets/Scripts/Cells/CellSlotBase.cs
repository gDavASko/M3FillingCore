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
    public IChip Chip => _chip;
    public ICover Cover => _cover;
    public IGenerator Generator => _generator;
    public CellConfig Info => _info;
    public bool CanPutChip => !_info.IsEmptySlot && _cover == null && _chip == null;

    public Action<ICellSlot> OnCellFree { get; set; }

    public Action OnReleased { get; set; }

    public void Init(IChip chip, ICover cover, IGenerator generator, Vector3 position, CellConfig info)
    {
        _info = info;
        transform.position = position;
        SetChip(chip, false, null);
        SetCover(cover);

        _generator = generator;

        _mainBG.gameObject.SetActive(!info.IsEmptySlot);

        if(_emptyBG != null)
            _emptyBG.gameObject.SetActive(info.IsEmptySlot);
    }

    public void Affect(System.Action OnAffectComplete)
    {
        if (_chip == null)
            return;

        var chip = _chip;
        _chip = null;

        chip.SetSlot(null);
        chip.DestroyAnimated(OnAffectComplete);
    }

    public void AffectAsNear(System.Action OnAffectComplete)
    {
        if(_cover != null)
            _cover.DealDamage(() =>
            {
                _cover = null;
                OnAffectComplete?.Invoke();
            });
    }

    public void SetChip(IChip chip, bool withAnimation, System.Action OnSetComplete)
    {
        OnChipLose();

        _chip = chip;

        if (_chip == null)
            return;

        _chip.transform.gameObject.SetActive(true);
        _chip.SetSlot(this);
        _chip.transform.SetParent(_chipTrs);
        _chip.transform.localScale = Vector3.one;

        if (withAnimation)
        {
            _chip.MoveAnimated(this, OnSetComplete);
        }
        else
        {
            _chip.transform.localPosition = Vector3.zero;
            OnSetComplete?.Invoke();
        }
    }

    public void ForgetChip()
    {
        _chip = null;
    }

    public void SetCover(ICover cover)
    {
        _cover = cover;

        if (_cover == null)
            return;

        _cover.transform.gameObject.SetActive(true);
        _cover.transform.SetParent(_coverTrs);
        _cover.transform.localPosition = Vector3.zero;
        _cover.transform.localScale = Vector3.one;
    }

    public void Release()
    {
        ReleaseComponents();
        gameObject.SetActive(false);

        OnReleased?.Invoke();

        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        Debug.LogError($"[{name}] DestroySelf!");
        ReleaseComponents();
        Destroy(this.gameObject);
    }

    public void SetPoolReleaser(IPoolReleaser<ICellSlot> pool)
    {
        //_pool = pool;
    }

    private void OnChipLose()
    {
        if (_chip != null)
        {
            var chip = _chip;
            _chip = null;

            chip.Release();
            chip = null;
        }
    }

    private void ReleaseComponents()
    {
        if (_chip != null)
        {
            _chip.Release();
            _chip = null;
        }

        if (_cover != null)
        {
            _cover.Release();
            _cover = null;
        }

        if (_generator != null)
        {
            _generator.Release();
            _generator = null;
        }
    }
}