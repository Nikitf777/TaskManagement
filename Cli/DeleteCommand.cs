using System;
using DotMake.CommandLine;
using TaskManagement.Repositories;

namespace TaskManagement.Cli;

[CliCommand(Description = "Delete a task by id.", Parent = typeof(RootCliCommand))]
public class DeleteCommand
{
	[CliArgument(Description = "The id of the task you want to delete.")]
	public required long Id { get; set; }

	public void Run()
	{
		var taskRepo = new TaskRepository(LocalStorageManager.ConnectionString);
		try {
			taskRepo.Delete(this.Id);
			Console.WriteLine("The task was successfully deleted.");
		} catch (Exception e) {
			Console.Error.WriteLine(e.Message);
		}
	}
}
