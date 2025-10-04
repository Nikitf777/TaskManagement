using Dapper;
using Microsoft.Data.Sqlite;
using TaskManagement.Models;
using TaskManagement.Exceptions;
using System;
using System.Collections.Generic;
using Polly.Retry;
using Polly;
using System.Linq;

namespace TaskManagement.Repositories;

public class TaskRepository(string conn)
{
	// TODO: Create a "SqliteConnectionRetryPolicy" to handle issues when opening a connection.
	private static readonly RetryPolicy QueryRetryPolicy;
	private readonly string connectionString = conn;

	static TaskRepository()
	{
		QueryRetryPolicy = Policy
			.Handle<SqliteException>(ex =>
				ex.SqliteErrorCode is
					(int)SqliteErrorCodes.SQLITE_BUSY or
					(int)SqliteErrorCodes.SQLITE_LOCKED)
			.WaitAndRetry(
				retryCount: 3,
				sleepDurationProvider: (retryCount) =>
					TimeSpan.FromMilliseconds(100)
			);
	}

	public long Create(TaskDomain task)
	{
		using var connection = new SqliteConnection(this.connectionString);
		connection.Open();
		var sqlQuery = $"insert into {TaskDb.TableName} ({nameof(TaskDomain.Title)}, {nameof(TaskDomain.Description)}, {nameof(TaskDomain.CreatedAt)}) values (@{nameof(TaskDomain.Title)}, @{nameof(TaskDomain.Description)}, @{nameof(TaskDomain.CreatedAt)})";
		var result = QueryRetryPolicy.ExecuteAndCapture(() => {
			try {
				if (connection.Execute(sqlQuery, new TaskDb(task)) == 0) {
					throw new TaskCreationException($"Failed creating a task with id {task.Id}.");
				}
				return connection.ExecuteScalar<long>("select last_insert_rowid()");
			} catch (SqliteException e) {
				LocalStorageManager.HandleSQLiteException(e);
				throw;
			}
		});

		result.ThrowIfException();

		return result.Result;
	}

	public void Delete(long id)
	{
		using var connection = new SqliteConnection(this.connectionString);
		connection.Open();
		var sqlQuery = $"delete from {TaskDb.TableName} where {nameof(TaskDomain.Id)} = @{nameof(id)}";
		var result = QueryRetryPolicy.ExecuteAndCapture(() => {
			try {
				if (connection.Execute(sqlQuery, new { id }) == 0) {
					throw new TaskNotExistException($"A task with id {id} does not exist.");
				}
			} catch (SqliteException e) {
				LocalStorageManager.HandleSQLiteException(e);
				throw;
			} catch (Exception) {
				throw;
			}
		});

		result.ThrowIfException();
	}

	public List<TaskDb> GetTasks()
	{
		using var connection = new SqliteConnection(this.connectionString);
		connection.Open();
		var result = QueryRetryPolicy.ExecuteAndCapture(() => {
			try {
				return connection.Query<TaskDb>($"select * from {TaskDb.TableName}").ToList();
			} catch (SqliteException e) {
				LocalStorageManager.HandleSQLiteException(e);
				throw;
			} catch (Exception) {
				throw;
			}
		});

		return result.ThrowIfExceptionAndReturn() ?? [];
	}

	public void UpdateCompletionStatus(long id, bool status)
	{
		using var connection = new SqliteConnection(this.connectionString);
		connection.Open();
		var sqlQuery = $"update {TaskDb.TableName} set {nameof(TaskDomain.IsCompleted)} = @{nameof(status)} where {nameof(TaskDomain.Id)} = @{nameof(id)}";
		var result = QueryRetryPolicy.ExecuteAndCapture(() => {
			try {
				if (connection.Execute(sqlQuery, new { id, status = Convert.ToInt64(status) }) == 0) {
					throw new TaskNotExistException($"A task with id {id} does not exist.");
				}
			} catch (SqliteException e) {
				LocalStorageManager.HandleSQLiteException(e);
				throw;
			} catch (Exception) {
				throw;
			}
		});

		result.ThrowIfException();
	}
}
