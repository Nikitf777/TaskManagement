using System;

namespace TaskManagement.Exceptions;

public class NoLocalDirectoryException(string? message) : Exception(message);
public class IncorrectDbFileException(string? message) : Exception(message);
public class TaskNotExistException(string? message) : Exception(message);
public class TaskCreationException(string? message) : Exception(message);