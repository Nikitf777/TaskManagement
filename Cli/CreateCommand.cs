using DotMake.CommandLine;
using System;
using TaskManagement.Repositories;
using TaskManagement.Models;
using TaskManagement.Exceptions;

namespace TaskManagement.Cli;

[CliCommand(Description = "Create and save a new task.", Parent = typeof(RootCliCommand))]
public class CreateCommand
{
	[CliArgument(Description = "The name of the task. Must be a non-empty string.")]
	public required string Title { get; set; }
	[CliArgument(Description = "The description of the task. Leave empty for no description", Required = false)]
	public string? Description { get; set; } = null;

	public void Run()
	{
		var taskRepo = new TaskRepository(LocalStorageManager.ConnectionString);
		var createdAt = DateTime.UtcNow;
		RootCliCommand.RepositoryAccessRetryPolicy.Execute(() => {
			try {
				var id = taskRepo.Create(new TaskDomain(this.Title, this.Description ?? "", createdAt));
				Console.WriteLine($"A task with id {id} is successfully created.");
			} catch (TaskCreationException e) {
				Console.Error.WriteLine(e);
			}
		});
	}
}
