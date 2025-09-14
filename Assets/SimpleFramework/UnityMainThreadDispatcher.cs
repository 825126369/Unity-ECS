using System;
using System.Collections.Concurrent; // 使用 ConcurrentQueue 更高效
using UnityEngine;

/// <summary>
/// 单例 MonoBehaviour，用于在主线程执行从其他线程提交的 Action。
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{
    public static UnityMainThreadDispatcher Instance { get; private set; }
    private readonly ConcurrentQueue<Action> _actionQueue = new ConcurrentQueue<Action>();
    private int _queuedActionsCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 防止场景切换时被销毁
        Debug.Log("UnityMainThreadDispatcher initialized.");
    }

    /// <summary>
    /// 从任何线程调用此方法，将一个 Action 加入队列，等待在主线程执行。
    /// </summary>
    /// <param name="action">要在主线程执行的委托</param>
    public void Enqueue(Action action)
    {
        if (action == null)
        {
            Debug.LogWarning("Attempted to enqueue a null action.");
            return;
        }

        _actionQueue.Enqueue(action);
        _queuedActionsCount++; // 用于调试，非必需
    }

    private void Update()
    {
        // 在主线程的 Update 中，处理队列中的所有 Action
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

        // 可选：调试日志
        if (processedCount > 0)
        {
            _queuedActionsCount -= processedCount;
            // Debug.Log($"Processed {processedCount} actions. {_queuedActionsCount} remaining.");
        }
    }

    // 可选：提供一个清理方法（通常不需要，因为队列会自动清空）
    public void Clear()
    {
        while (_actionQueue.TryDequeue(out _)) { }
        _queuedActionsCount = 0;
    }

    // 可选：检查队列是否为空
    public bool IsEmpty => _actionQueue.IsEmpty;
}