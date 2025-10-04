using Polly;

internal static class PolicyResultExtension
{
	public static void ThrowIfException<T>(this PolicyResult<T> result)
	{
		if (result.FinalException is not null) {
			throw result.FinalException;
		}
	}
	public static void ThrowIfException(this PolicyResult result)
	{
		if (result.FinalException is not null) {
			throw result.FinalException;
		}
	}
	public static T ThrowIfExceptionAndReturn<T>(this PolicyResult<T> result)
	{
		if (result.FinalException is not null) {
			throw result.FinalException;
		}
		return result.Result;
	}
}