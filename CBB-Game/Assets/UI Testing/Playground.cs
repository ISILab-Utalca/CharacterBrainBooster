using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Playground : MonoBehaviour
{
    private Action testAction = null;
    public List<int> values = new();
    [ContextMenu("Call async method but not await on Editor loop")]
    public async Task NotAwaitedAsync()
    {
        await Task.Run(() =>
        {
            try
            {
                Thread.Sleep(1000);
                Debug.Log("Async sleep terminated");
                throw new Exception("OWO Exception not handled");
            }
            catch (Exception ex)
            {
                Debug.Log("Error Handled in thread:" + ex);
            }
        });
        Debug.Log("Oh no, forget to await the previous call");
    }
    [ContextMenu("Call sync method with exception")]
    public void WaitedSync()
    {
        try
        {
            Task.Run(() => { Thread.Sleep(1000); Debug.Log("Async sleep terminated"); throw new Exception("OWO Exception not handled"); });

        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        JustThrowExcept();
        Debug.Log("Oh no, forget to await the previous call");
    }

    private void JustThrowExcept()
    {
        throw new Exception("OWO Dummy exception");
    }
    private void Awake()
    {
        testAction += DummyActionMethod;
        Thread t = new(DummyThreadMethod);
        t.Start();
        t.Join();
    }

    private void DummyActionMethod()
    {
        Debug.Log("Action method called");
    }

    private void DummyThreadMethod()
    {
        Debug.Log("1");
        Thread.Sleep(1000);
        testAction.Invoke();
    }
    private void OnDestroy()
    {
        testAction -= DummyActionMethod;
    }
}
