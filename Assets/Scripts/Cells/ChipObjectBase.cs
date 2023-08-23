using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class ChipObjectBase : MonoBehaviour, IChip
{
    [SerializeField] private string id = "chip";

    private IPoolReleaser<IChip> _pool = null;
    private IChipMoveAnimtor _moveAnimator = null;
    private IDestroyAnimator _destroyAnimator = null;
    private ICellSlot _slot = null;

    private Vector3 _initScale = Vector3.one;
    private WaitForFixedUpdate _waiter = null;
    private System.Action OnMoveCompleteCallback = null;
    private System.Action OnDestroyCompleteCallback = null;

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
        gameObject.SetActive(false);
        OnReleased?.Invoke();

        _slot = null;

        if (_pool != null)
        {
            _pool.ReleaseComponent(this);
        }
        else
        {
            DestroySelf();
        }
    }

    public void SetPoolReleaser(IPoolReleaser<IChip> pool)
    {
        _pool = pool;
    }

    public void SetSlot(ICellSlot slot)
    {
        _slot = slot;
    }

    public void MoveAnimated(ICellSlot toSlot, System.Action OnMoveComplete)
    {
        OnMoveCompleteCallback = OnMoveComplete;
        if (_moveAnimator != null)
        {
            _moveAnimator.MoveTo(toSlot);
        }
        else
        {
            _waiter = new WaitForFixedUpdate();
            if(gameObject.activeSelf)
                StartCoroutine(MoveToSlotPlay(toSlot));
        }
    }

    public void DestroyAnimated(System.Action OnDestroyComplete)
    {
        OnDestroyCompleteCallback = OnDestroyComplete;
        if (_destroyAnimator != null)
        {
            _destroyAnimator.AnimatedDestroy();
        }
        else
        {
            _waiter = new WaitForFixedUpdate();
            if(gameObject.activeSelf)
                StartCoroutine(DestroyAnimatedPlay());
        }
    }

    public void TweenScale()
    {
        if(gameObject.activeSelf)
            StartCoroutine(SimpleTweenPlay());
    }

    public void DestroySelf()
    {
        Debug.LogError($"[{name}] DestroySelf!");
        Destroy(this.gameObject);
    }

    private void OnEndMove(ICellSlot toSlot)
    {
        OnMoveCompleteCallback?.Invoke();
        OnMoveCompleteCallback = null;
    }

    private IEnumerator MoveToSlotPlay(ICellSlot toSlot)
    {
        Vector3 initPos = transform.localPosition;
        Vector3 targetPos = Vector3.zero;

        for (float i = 0; i <= 1f; i += 0.15f)
        {
            transform.localPosition = Vector3.Lerp(initPos, targetPos, i);
            yield return _waiter;
        }

        transform.localPosition = targetPos;

        OnEndMove(toSlot);
    }

    private void OnAnimatedDestroyComplete()
    {
        Release();
        OnDestroyCompleteCallback?.Invoke();
        OnDestroyCompleteCallback = null;
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