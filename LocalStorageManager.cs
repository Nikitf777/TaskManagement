using System;
using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;
using TaskManagement.Exceptions;
using TaskManagement.Models;

namespace TaskManagement;

public static class LocalStorageManager
{
	public const string AppDataFolderName = "TaskManagement";
	public static string AppDataFolderPath => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppDataFolderName);
	public const string SqliteFileName = "db.sqlite";
	public static string DbPath => Path.Join(AppDataFolderPath, SqliteFileName);
	public static string ConnectionString => $"Data Source={DbPath}";
	public static string CreateTableQuery => $"create table \"{TaskDb.TableName}\" (\n\"{nameof(TaskDomain.Id)}\"	integer not null primary key autoincrement unique,\n\"{nameof(TaskDomain.Title)}\"	text not null check(length(\"{nameof(TaskDomain.Title)}\") > 0),\n\"{nameof(TaskDomain.Description)}\"	text not null default \"\",\n\"{nameof(TaskDomain.IsCompleted)}\"	integer not null default 0 check(\"{nameof(TaskDomain.IsCompleted)}\" in (0, 1)),\n\"{nameof(TaskDomain.CreatedAt)}\"	integer not null\n);";

	public static void InitializeDb()
	{
		var connection = new SqliteConnection(ConnectionString);
		connection.Open();
		_ = connection.Query(CreateTableQuery);
	}

	public static void HandleSQLiteException(SqliteException e)
	{
		switch (e.SqliteErrorCode) {
			case (int)SqliteErrorCodes.SQLITE_ERROR: {
				if (e.Message == $"SQLite Error 1: 'no such table: {TaskDb.TableName}'.") {
					InitializeDb();
					throw new IncorrectDbFileException($"The database file didn't contain the necessary '{TaskDb.TableName}' table, so it was created.");
				}
				break;
			}
			case (int)SqliteErrorCodes.SQLITE_CORRUPT: {
				InitializeDb();
				break;
			}
			case (int)SqliteErrorCodes.SQLITE_CANTOPEN: {
				_ = Directory.CreateDirectory(AppDataFolderPath);
				InitializeDb();
				throw new NoLocalDirectoryException($"The local storage directory '{AppDataFolderPath}' didn't exist, so it was created.");
			}
			default: {
				throw e;
			}
		}
	}
}