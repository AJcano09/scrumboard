namespace ScrumBoard.Domain.Exceptions;

public class ProjectValidationException(string message) : Exception(message);