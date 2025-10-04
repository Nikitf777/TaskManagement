using System;

namespace TaskManagement.Models;

public readonly struct TaskDb(string title, long createdAt)
{
	public const string TableName = "Tasks";

	public long Id { get; }
	public string Title { get; } = title;
	public string Description { get; } = string.Empty;
	public long IsCompleted { get; } = createdAt;
	public long CreatedAt { get; }

	public TaskDb(TaskDomain task) : this(task.Title, new DateTimeOffset(task.CreatedAt).ToUnixTimeSeconds())
	{
		this.Id = task.Id;
		this.Description = task.Description;
		this.IsCompleted = Convert.ToInt64(task.IsCompleted);
		this.CreatedAt = new DateTimeOffset(task.CreatedAt).ToUnixTimeSeconds();
	}
}

public readonly struct TaskDomain(string title, DateTime createdAt)
{
	public long Id { get; }
	public string Title { get; } = title;
	public string Description { get; } = string.Empty;
	public bool IsCompleted { get; } = false;
	public DateTime CreatedAt { get; } = createdAt;

	public TaskDomain(string title, string description, DateTime createdAt) : this(title, createdAt)
	{
		this.Description = description;
	}

	public TaskDomain(TaskDb task) : this(task.Title, DateTimeOffset.FromUnixTimeSeconds(task.CreatedAt).UtcDateTime)
	{
		this.Id = task.Id;
		this.Description = task.Description;
		this.IsCompleted = Convert.ToBoolean(task.IsCompleted);
	}
}