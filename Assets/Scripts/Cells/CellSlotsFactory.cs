using UnityEngine;
using UnityEngine.Pool;

public class CellSlotsFactory : MonoBehaviour, ICellSlotsFactory
{
    [SerializeField] private GameObject _slotPrefab = default;
    [SerializeField] private Transform _slotsRoot = null;
    [SerializeField] private Vector2 _scaleMultPositions = Vector2.one;
    [SerializeField] private float _slotsZCoordinate = 0f;

    private IObjectPool<ICellSlot> _pool = null;

    private IComponentFactory<IChip> _chipFactory = null;
    private IComponentFactory<ICover> _coverFactory = null;
    private IComponentFactory<IGenerator> _generatorFactory = null;

    public bool Inited { get; private set; } = false;

    public void Construct(IComponentFactory<IChip> componentFactory, IComponentFactory<ICover> coverFactory, IComponentFactory<IGenerator> generatorFactory)
    {
        _chipFactory = componentFactory;
        _coverFactory = coverFactory;
        _generatorFactory = generatorFactory;

        _pool = new ObjectPool<ICellSlot>(OnCreateSlot, OnGetSlot,
            OnReleaseSlot, OnDestroySlot, false, 64);

        Inited = true;
    }

    public void CreateSlots(CellsSlotsConfig configs)
    {
        foreach (var config in configs.SlotCells)
        {
            CreateComponents(config);
        }
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
            cell.Init(_chipFactory.GetComponentByID(slotInfo.FillType),
                _coverFactory.GetComponentByID(slotInfo.CoverType),
                _generatorFactory.GetComponentByID(slotInfo.GeneratorType),
                CalculateCellPosition(slotInfo.Position),
                slotInfo);
            cell.transform.gameObject.SetActive(true);
        }
    }

    private Vector3 CalculateCellPosition(Vector2 coordinates)
    {
        Vector3 res = default;

        res.z = _slotsZCoordinate;
        res.x = coordinates.x * _scaleMultPositions.x;
        res.y = coordinates.y * _scaleMultPositions.y;

        return res;
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
        slot.SetPoolContainer(_pool);
    }
    #endregion IPoolObject
}