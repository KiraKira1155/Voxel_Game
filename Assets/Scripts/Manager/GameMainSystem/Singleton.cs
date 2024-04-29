using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected virtual bool DestroyTargetGameObj => false;
    public static T I { get; private set; } = null;

    public static bool IsValid() => I != null;

    protected void Init()
    {
        if(I == null)
        {
            I = this as T;
            return;
        }
        if (DestroyTargetGameObj)
        {
            Destroy(gameObject);
        }
        else{
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if(I == this)
        {
            I = null;
        }
        OnRelease();
    }

    /// <summary>
    /// îhê∂ÉNÉâÉXópOnDestroy
    /// </summary>
    protected virtual void OnRelease()
    {

    }
}
