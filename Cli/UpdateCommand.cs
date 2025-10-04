using System;
using DotMake.CommandLine;
using TaskManagement.Exceptions;
using TaskManagement.Repositories;

namespace TaskManagement.Cli;

[CliCommand(Description = "Change the completion status of a task by its id.", Parent = typeof(RootCliCommand))]
public class UpdateCommand
{
	[CliArgument(Description = "The id of the task to update.")]
	public required long Id { get; set; }
	[CliArgument(Description = "The new completion status.")]
	public bool Status { get; set; } = true;

	public void Run()
	{
		var repo = new TaskRepository(LocalStorageManager.ConnectionString);
		RootCliCommand.RepositoryAccessRetryPolicy.Execute(() => {
			try {
				repo.UpdateCompletionStatus(this.Id, this.Status);
				Console.WriteLine("The completion status was updated successfully.");
			} catch (Exception e) {
				Console.Error.WriteLine(e.Message);
			}
		});
	}
}
