namespace ScrumBoard.Domain.Exceptions;

public class TaskValidationException(string message) : Exception(message);