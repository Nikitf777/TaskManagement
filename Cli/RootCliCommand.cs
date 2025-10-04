using System;
using DotMake.CommandLine;
using Polly;
using Polly.Retry;
using TaskManagement.Exceptions;

namespace TaskManagement.Cli;

[CliCommand(Description = "A simple task management app.")]
public class RootCliCommand
{
	public static readonly RetryPolicy RepositoryAccessRetryPolicy;

	static RootCliCommand()
	{
		RepositoryAccessRetryPolicy = Policy
			.Handle<IncorrectDbFileException>()
			.WaitAndRetry(
				retryCount: 2,
				sleepDurationProvider: (retryCount) =>
					TimeSpan.FromMilliseconds(10)
			);
	}
}