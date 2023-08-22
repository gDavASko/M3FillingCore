using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class CellCoverBase : MonoBehaviour, ICover
{
    [SerializeField] private string _id = "cover";
    [SerializeField] private int _damageCount = 1;

    private IObjectPool<ICover> _pool = null;
    private IDestroyAnimator _destroyAnimator = null;
    private IDamageAnimator _damageAnimator = null;

    private System.Action OnDestroyCallback = null;

    private int _dmgCounter = 0;
    private WaitForFixedUpdate _waiter = null;
    private Vector3 initScale = Vector3.one;

    public string ID => _id;
    public Action OnReleased { get; set; }


    private void Awake()
    {
        initScale = transform.localScale;
        _dmgCounter = _damageCount;

        _destroyAnimator = GetComponent<IDestroyAnimator>();
        if (_destroyAnimator != null)
        {
            _destroyAnimator.OnDestroyComplete += OnEndDestroy;
        }

        _damageAnimator = GetComponent<IDamageAnimator>();
        if (_damageAnimator != null)
        {
            _damageAnimator.OnDamageComplete += OnDamageComplete;
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

    public void SetPoolContainer(IObjectPool<ICover> pool)
    {
        _pool = pool;

        transform.localScale = initScale;
        _dmgCounter = _damageCount;
    }

    public void DealDamage(System.Action OnDestroyCallback)
    {
        _dmgCounter--;
        if (_dmgCounter <= 0)
        {
            DestroyAnimated();
            this.OnDestroyCallback = OnDestroyCallback;
        }
        else
        {
            DamageAnimated();
        }
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    private void DestroyAnimated()
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

    private IEnumerator DestroyAnimatedPlay()
    {
        Vector3 resScale = initScale * 1.1f;

        for (float i = 0; i <= 1; i += 0.1f)
        {
            transform.localScale = Vector3.Lerp(initScale, resScale, i);
            yield return _waiter;
        }

        OnEndDestroy();
    }

    private void OnEndDestroy()
    {
        Release();
        OnDestroyCallback?.Invoke();
    }

    private void DamageAnimated()
    {
        if (_damageAnimator != null)
        {
            _damageAnimator.AnimatedDealDamage(_dmgCounter/(float)_damageCount);
        }
        else
        {
            _waiter = new WaitForFixedUpdate();
            StartCoroutine(DamageAnimatedPlay());
        }
    }

    private IEnumerator DamageAnimatedPlay()
    {
        Vector3 resScale = initScale * 1.1f;

        for (float i = 0; i <= 1; i += 0.1f)
        {
            transform.localScale = Vector3.Lerp(initScale, resScale, i);
            yield return _waiter;
        }

        for (float i = 0; i <= 1; i += 0.1f)
        {
            transform.localScale = Vector3.Lerp(resScale, initScale, i);
            yield return _waiter;
        }

        OnDamageComplete();
    }

    private void OnDamageComplete()
    {

    }
}