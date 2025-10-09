#pragma warning disable CA1822 // Mark members as static

using DotMake.CommandLine;
using TaskManagement.Repositories;
using Spectre.Console;
using TaskManagement.Models;
using System.CommandLine.Rendering;
using TaskManagement.Exceptions;
using System;

namespace TaskManagement.Cli;

[CliCommand(Description = "List all the created tasks.", Parent = typeof(RootCliCommand))]
public class ListCommand
{
	public void Run()
	{
		var repo = new TaskRepository(LocalStorageManager.ConnectionString);
		RootCliCommand.RepositoryAccessRetryPolicy.Execute(() => {
			try {
				var tasks = repo.GetTasks();
				var table = new Table();
				_ = table.AddColumns(
					nameof(TaskDomain.Id),
					nameof(TaskDomain.Title),
					nameof(TaskDomain.Description),
					nameof(TaskDomain.IsCompleted),
					nameof(TaskDomain.CreatedAt)
				);

				foreach (var taskDb in tasks) {
					var task = new TaskDomain(taskDb);
					_ = table.AddRow(task.Id.ToString(new ConsoleFormatInfo()), task.Title, task.Description, task.IsCompleted.ToString(), task.CreatedAt.ToString(new ConsoleFormatInfo()));
				}

				AnsiConsole.Write(table);
			} catch (IncorrectDbFileException) {
				Console.WriteLine("Tasks database was initialized.");
				throw;
			} catch (Exception e) {
				Console.Error.Write(e.Message);
			}
		});
	}
}

