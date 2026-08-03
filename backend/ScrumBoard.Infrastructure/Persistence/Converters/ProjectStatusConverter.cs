using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScrumBoard.Domain.Enums;

namespace ScrumBoard.Infrastructure.Persistence.Converters;

public class ProjectStatusConverter() : ValueConverter<ProjectStatus, string>(status => status.Name,
    value => ProjectStatus.FromName(value));