using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class ObjectWorkQueue<T>
{
	protected Queue<T> queue = new Queue<T>();

	protected HashSet<T> containerTest = new HashSet<T>();

	public string queueName;

	public long warnTime;

	public long totalProcessed;

	public double totalTime;

	public int queueProcessedLast;

	public double lastMS;

	public int hashsetMaxLength;

	public int queueLength => queue.Count;

	public ObjectWorkQueue()
	{
		queueName = GetType().FullName;
	}

	public void Add(T entity)
	{
		if (!Contains(entity) && ShouldAdd(entity))
		{
			queue.Enqueue(entity);
			containerTest.Add(entity);
			hashsetMaxLength = Mathf.Max(containerTest.Count, hashsetMaxLength);
		}
	}

	public void Clear()
	{
		queue.Clear();
		queue = new Queue<T>();
		containerTest.Clear();
		if (hashsetMaxLength > 256)
		{
			containerTest = new HashSet<T>();
			hashsetMaxLength = 0;
		}
	}

	public bool Contains(T entity)
	{
		return containerTest.Contains(entity);
	}

	protected abstract void RunJob(T entity);

	public void RunQueue(double maximumMilliseconds)
	{
		if (queue.Count == 0)
		{
			return;
		}
		SortQueue();
		Stopwatch stopwatch = Stopwatch.StartNew();
		queueProcessedLast = 0;
		while (queue.Count > 0)
		{
			queueProcessedLast++;
			totalProcessed++;
			T val = queue.Dequeue();
			containerTest.Remove(val);
			if (val != null)
			{
				RunJob(val);
			}
			if (!(stopwatch.Elapsed.TotalMilliseconds < maximumMilliseconds))
			{
				break;
			}
		}
		lastMS = stopwatch.Elapsed.TotalMilliseconds;
		totalTime += lastMS;
		if (queue.Count == 0)
		{
			Clear();
		}
	}

	protected virtual bool ShouldAdd(T entity)
	{
		return true;
	}

	protected virtual void SortQueue()
	{
	}
}
