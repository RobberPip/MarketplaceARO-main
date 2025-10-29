using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Shared;

public static class FluentValidationExtension {
  public static void AddFluentValidation(this IServiceCollection services, Type assemblyType) {
    services.AddFluentValidationAutoValidation();
    services.AddFluentValidationRulesToSwagger();
    services.AddValidatorsFromAssemblyContaining(assemblyType);
  }
}