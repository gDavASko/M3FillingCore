using System;
using UnityEngine;
using UnityEngine.Pool;

public class CellSlotsFactory : MonoBehaviour, ICellSlotsFactory
{
    [SerializeField] private GameObject _slotPrefab = default;
    [SerializeField] private Transform _slotsRoot = null;
    [SerializeField] private float _slotsZCoordinate = 0f;

    private IObjectPool<ICellSlot> _pool = null;

    private IPooledCustomFactory<IChip> _chipFactory = null;
    private IPooledCustomFactory<ICover> _coverFactory = null;
    private IPooledCustomFactory<IGenerator> _generatorFactory = null;

    private SlotEvents _slotEvents = null;
    private GameParameters _parameters = null;
    private Vector2 _scaleMultPositions = Vector2.one;
    private Vector3 _positionOffset = Vector2.zero;

    public bool Inited { get; private set; } = false;

    public void Construct(IPooledCustomFactory<IChip> componentFactory, IPooledCustomFactory<ICover> coverFactory,
        IPooledCustomFactory<IGenerator> generatorFactory, SlotEvents slotEvents, GameParameters parameters)
    {
        _chipFactory = componentFactory;
        _coverFactory = coverFactory;
        _generatorFactory = generatorFactory;

        _slotEvents = slotEvents;
        _parameters = parameters;

        _pool = new ObjectPool<ICellSlot>(OnCreateSlot, OnGetSlot,
            OnReleaseSlot, OnDestroySlot, false, 64);

        Inited = true;
    }

    public void CreateSlots(CellsSlotsConfig configs)
    {
        CalculateCellVariables(configs);

        foreach (var config in configs.SlotCells)
        {
            CreateComponents(config);
        }

        _slotEvents.OnSlotsViewCreated?.Invoke();
    }

    public void ReleaseSlots()
    {
        _pool.Clear();
    }

    private void CreateComponents(CellConfigs config)
    {
        foreach (var slotInfo in config.Cells)
        {
            var cell = _pool.Get();

            var chip = _chipFactory.GetComponentByID(slotInfo.FillType);
            if(chip != null)
                chip.SetPoolReleaser(_chipFactory);

            var cover = _coverFactory.GetComponentByID(slotInfo.CoverType);
            if(cover != null)
                cover.SetPoolReleaser(_coverFactory);

            var generator = _generatorFactory.GetComponentByID(slotInfo.GeneratorType);
            if(generator != null)
                generator.SetPoolReleaser(_generatorFactory);

            cell.Init(chip, cover, generator, CalculateCellPosition(slotInfo.Position), slotInfo);

            _slotEvents.OnAddedSlot?.Invoke(cell);

            cell.transform.gameObject.SetActive(true);
        }
    }

    private Vector3 CalculateCellPosition(Vector2 coordinates)
    {
        Vector3 res = default;

        res.z = _slotsZCoordinate;
        res.x = (coordinates.x ) * _scaleMultPositions.x;
        res.y = (coordinates.y) * _scaleMultPositions.y;

        res += _positionOffset;

        return res;
    }

    private void CalculateCellVariables(CellsSlotsConfig configs)
    {
        _positionOffset = (configs.FieldSize * -0.5f + new Vector2(0.5f, 0.5f));
        _scaleMultPositions = new Vector2(1f, 1f);
    }

    #region IPoolObject
    private ICellSlot OnCreateSlot()
    {
        ICellSlot go = Instantiate(_slotPrefab, _slotsRoot).GetComponent<ICellSlot>();
        return go;
    }

    private void OnDestroySlot(ICellSlot slot)
    {
        slot.DestroySelf();
    }

    private void OnReleaseSlot(ICellSlot slot)
    {
        slot.transform.gameObject.SetActive(false);
    }

    private void OnGetSlot(ICellSlot slot)
    {
        //slot.SetPoolReleaser(_pool);
    }
    #endregion IPoolObject
}