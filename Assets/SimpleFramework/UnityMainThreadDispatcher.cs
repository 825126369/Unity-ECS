using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;

/// <summary>
/// 单例 MonoBehaviour，用于在主线程执行从其他线程提交的 Action。
/// </summary>
public class UnityMainThreadDispatcher : SingleTonMonoBehaviour<UnityMainThreadDispatcher>
{
    private readonly Dictionary<int, List<InnerListener>> listenerDic = new Dictionary<int, List<InnerListener>>();
    private readonly ConcurrentQueue<InnerEvent> mInnerEventQueue = new ConcurrentQueue<InnerEvent>();
    public struct InnerListener
    {
        public bool once;
        public Action<object> mFunc;
    }

    public struct InnerEvent
    {
        public int nId;
        public object mData;
    }

    public void AddListener(int nId, Action<object> action, bool once = false)
    {
        if (action == null)
        {
            Debug.LogWarning("Attempted to enqueue a null action.");
            return;
        }

        InnerListener mInnerListener = new InnerListener();
        mInnerListener.mFunc = action;
        mInnerListener.once = once;

        lock (listenerDic)
        {
            List<InnerListener> mList = null;
            if (!listenerDic.TryGetValue(nId, out mList))
            {
                mList = new List<InnerListener>();
                listenerDic.Add(nId, mList);
            }
            mList.Add(mInnerListener);
        }
    }

    public void RemoveListener(int nId, Action<object> mFunc)
    {
        if (mFunc == null)
        {
            Debug.LogWarning("Attempted to enqueue a null action.");
            return;
        }

        lock (listenerDic)
        {
            List<InnerListener> mList = null;
            if (listenerDic.TryGetValue(nId, out mList))
            {
                var l = mList.Count;
                for (int i = l - 1; i >= 0; i--)
                {
                    if (mList[i].mFunc == mFunc)
                    {
                        mList.RemoveAt(i);
                    }
                }
            }
        }
    }
    
    public void Fire(int nId, object args = null)
    {
        InnerEvent mInnerListener = new InnerEvent();
        mInnerListener.nId = nId;
        mInnerListener.mData = args;
        mInnerEventQueue.Enqueue(mInnerListener);
    }

    private void Update()
    {
        int processedCount = 0;
        while (_actionQueue.TryDequeue(out Action action))
        {
            try
            {
                action.Invoke(); // 执行委托
            }
            catch (Exception e)
            {
                Debug.LogError($"Error executing action in UnityMainThreadDispatcher: {e}");
            }
            processedCount++;
        }
        
        if (processedCount > 0)
        {
            _queuedActionsCount -= processedCount;
        }
    }
    
    public void Clear()
    {
        while (_actionQueue.TryDequeue(out _)) { }
        _queuedActionsCount = 0;
    }
    
    public bool IsEmpty => _actionQueue.IsEmpty;
}