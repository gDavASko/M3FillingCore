using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class ChipObjectBase : MonoBehaviour, IChip, IPointerClickHandler
{
    [SerializeField] private string id = "chip";

    private IObjectPool<IChip> _pool = null;
    private IChipMoveAnimtor _moveAnimator = null;
    private IDestroyAnimator _destroyAnimator = null;
    private ICellSlot _slot = null;

    private Vector3 _initScale = Vector3.one;
    private WaitForFixedUpdate _waiter = null;

    public string ID => id;
    public Action OnReleased { get; set; }
    public Action<IChip> OnClick { get; set; }
    public bool Interactive { get; private set; } = true;

    private void Awake()
    {
        _initScale = transform.localScale;

        _moveAnimator = GetComponent<IChipMoveAnimtor>();
        if (_moveAnimator != null)
        {
            _moveAnimator.OnMoveComplete += OnEndMove;
        }

        _destroyAnimator = GetComponent<IDestroyAnimator>();
        if (_destroyAnimator != null)
        {
            _destroyAnimator.OnDestroyComplete += OnAnimatedDestroyComplete;
        }
    }

    public void Release()
    {
        Interactive = false;
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

    public void SetPoolContainer(IObjectPool<IChip> pool)
    {
        _pool = pool;
    }

    public void SetSlot(ICellSlot slot)
    {
        _slot = slot;
        Interactive = true;
        transform.localScale = _initScale;
    }

    public void MoveAnimated(ICellSlot toSlot)
    {
        Interactive = false;
        if (_moveAnimator != null)
        {
            _moveAnimator.MoveTo(toSlot);
        }
        else
        {
            _waiter = new WaitForFixedUpdate();
            StartCoroutine(MoveToTransformPlay(toSlot));
        }
    }

    public void DestroyAnimated()
    {
        Interactive = false;
        if (_destroyAnimator != null)
        {
            _destroyAnimator.AnimatedDestroy();
        }
        else
        {
            _waiter = new WaitForFixedUpdate();
            StartCoroutine(DestroyAnimatedPlay());
        }
    }

    public void DestroySelf()
    {
        Interactive = false;
        Destroy(this.gameObject);
    }

    private void OnEndMove(ICellSlot toSlot)
    {
        toSlot.SetChip(this);
        Interactive = true;
    }

    private IEnumerator MoveToTransformPlay(ICellSlot toSlot)
    {
        Vector3 initPos = transform.position;
        Vector3 targetPos = toSlot.transform.position;

        for (float i = 0; i <= 1; i += 0.01f)
        {
            transform.position = Vector3.Lerp(initPos, targetPos, i);
            yield return _waiter;
        }

        OnEndMove(toSlot);
    }

    private void OnAnimatedDestroyComplete()
    {
        Release();
    }

    private IEnumerator DestroyAnimatedPlay()
    {
        Vector3 resScale = _initScale * 1.1f;

        for (float i = 0; i <= 1; i += 0.01f)
        {
            transform.localScale = Vector3.Lerp(_initScale, resScale, i);
            yield return _waiter;
        }

        OnAnimatedDestroyComplete();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Interactive)
            OnClick?.Invoke(this);
    }
}