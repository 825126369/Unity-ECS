using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

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

    public void Init()
    {
        
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
        while (mInnerEventQueue.TryDequeue(out InnerEvent mEvent))
        {
            List<InnerListener> mList = null;
            if (listenerDic.TryGetValue(mEvent.nId, out mList))
            {
                int l = mList.Count;
                for (int i = l - 1; i >= 0; i--)
                {
                    InnerListener ml = mList[i];
                    ml.mFunc(mEvent.mData);
                    if (ml.once)
                    {
                        RemoveListener(mEvent.nId, ml.mFunc);
                    }
                }
            }
        }
    }
}