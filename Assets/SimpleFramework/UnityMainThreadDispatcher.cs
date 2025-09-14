using System;
using System.Collections.Concurrent; // ʹ�� ConcurrentQueue ����Ч
using UnityEngine;

/// <summary>
/// ���� MonoBehaviour�����������߳�ִ�д������߳��ύ�� Action��
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
        DontDestroyOnLoad(gameObject); // ��ֹ�����л�ʱ������
        Debug.Log("UnityMainThreadDispatcher initialized.");
    }

    /// <summary>
    /// ���κ��̵߳��ô˷�������һ�� Action ������У��ȴ������߳�ִ�С�
    /// </summary>
    /// <param name="action">Ҫ�����߳�ִ�е�ί��</param>
    public void Enqueue(Action action)
    {
        if (action == null)
        {
            Debug.LogWarning("Attempted to enqueue a null action.");
            return;
        }

        _actionQueue.Enqueue(action);
        _queuedActionsCount++; // ���ڵ��ԣ��Ǳ���
    }

    private void Update()
    {
        // �����̵߳� Update �У���������е����� Action
        int processedCount = 0;
        while (_actionQueue.TryDequeue(out Action action))
        {
            try
            {
                action.Invoke(); // ִ��ί��
            }
            catch (Exception e)
            {
                Debug.LogError($"Error executing action in UnityMainThreadDispatcher: {e}");
            }
            processedCount++;
        }

        // ��ѡ��������־
        if (processedCount > 0)
        {
            _queuedActionsCount -= processedCount;
            // Debug.Log($"Processed {processedCount} actions. {_queuedActionsCount} remaining.");
        }
    }

    // ��ѡ���ṩһ����������ͨ������Ҫ����Ϊ���л��Զ���գ�
    public void Clear()
    {
        while (_actionQueue.TryDequeue(out _)) { }
        _queuedActionsCount = 0;
    }

    // ��ѡ���������Ƿ�Ϊ��
    public bool IsEmpty => _actionQueue.IsEmpty;
}