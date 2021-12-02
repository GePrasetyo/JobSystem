using System.Threading;
using System.Collections;

public class JobThread
{
    private bool _isDone = false;
    private object _handle = new object();
    private Thread _thread = null;

    public bool WithReturn;

    public bool IsDone
    {
        get
        {
            bool tmp;
            lock (_handle)
            {
                tmp = _isDone;
            }
            return tmp;
        }
        set
        {
            lock (_handle)
            {
                _isDone = value;
            }
        }
    }

    public virtual void Start()
    {
        _thread = new Thread(Run);
        _thread.Start();
    }
    public virtual void Abort()
    {
        _thread.Abort();
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnFinished() { }

    public virtual bool Update()
    {
        if (IsDone)
        {
            OnFinished();
            return true;
        }
        return false;
    }

    public IEnumerator WaitFor()
    {
        while (!Update())
        {
            yield return null;
        }
    }

    private void Run()
    {
        ThreadFunction();
        IsDone = true;
    }
}
