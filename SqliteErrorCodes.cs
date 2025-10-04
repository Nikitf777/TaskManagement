#pragma warning disable CA1707 // Identifiers should not contain underscores

internal enum SqliteErrorCodes
{
	SQLITE_ERROR = 1,
	SQLITE_BUSY = 5,
	SQLITE_LOCKED = 6,
	SQLITE_CORRUPT = 11,
	SQLITE_CANTOPEN = 14,
}