using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class ChipObjectBase : MonoBehaviour, IChip
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
        transform.localScale = _initScale;
    }

    public void MoveAnimated(ICellSlot toSlot)
    {
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

    public void TweenScale()
    {
        StartCoroutine(SimpleTweenPlay());
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    private void OnEndMove(ICellSlot toSlot)
    {

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

        for (float i = 0; i <= 1; i += 0.2f)
        {
            transform.localScale = Vector3.Lerp(_initScale, resScale, i);
            yield return _waiter;
        }

        OnAnimatedDestroyComplete();
    }

    private IEnumerator SimpleTweenPlay()
    {
        Vector3 resScale = _initScale * 0.8f;

        for (float i = 0; i <= 1; i += 0.2f)
        {
            transform.localScale = Vector3.Lerp(_initScale, resScale, i);
            yield return _waiter;
        }

        for (float i = 0; i <= 1; i += 0.2f)
        {
            transform.localScale = Vector3.Lerp(resScale, _initScale, i);
            yield return _waiter;
        }
    }
}