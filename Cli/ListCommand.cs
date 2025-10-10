#pragma warning disable CA1822 // Mark members as static

using DotMake.CommandLine;
using TaskManagement.Repositories;
using Spectre.Console;
using TaskManagement.Models;
using System.CommandLine.Rendering;
using System;

namespace TaskManagement.Cli;

[CliCommand(Description = "List all the created tasks.", Parent = typeof(RootCliCommand))]
public class ListCommand
{
	public void Run()
	{
		var taskRepo = new TaskRepository(LocalStorageManager.ConnectionString);
		RootCliCommand.RepositoryAccessRetryPolicy.Execute(() => {
			try {
				var tasks = taskRepo.GetTasks();
				var table = new Table();
				foreach (var property in typeof(TaskDomain).GetProperties()) {
					_ = table.AddColumn(property.Name);
				}

				foreach (var taskDb in tasks) {
					var task = new TaskDomain(taskDb);
					_ = table.AddRow(task.Id.ToString(new ConsoleFormatInfo()), task.Title, task.Description, task.IsCompleted.ToString(), task.CreatedAt.ToLocalTime().ToString(new ConsoleFormatInfo()));
				}

				AnsiConsole.Write(table);
			} catch (Exception e) {
				Console.Error.Write(e.Message);
			}
		});
	}
}

