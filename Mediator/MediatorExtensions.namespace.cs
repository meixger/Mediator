using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

// Needs NuGet Microsoft.Extensions.DependencyModel

// ReSharper disable once CheckNamespace
namespace MediatR;

public static partial class MediatorExtensions
{
    /// <summary>
    /// Uses `DependencyContext` from `Microsoft.Extensions.DependencyModel` to scan for types from namespaces starting with <param name="namespace">namespace</param>
    /// </summary>
    /// <param name="namespace">Assembly FullName must start with</param>
    public static void AddMediatR(this IServiceCollection services, string @namespace)
    {
        var assemblies = DependencyContext.Default!.GetDefaultAssemblyNames().Where(assembly => assembly.FullName.StartsWith(@namespace)).Select(Assembly.Load).ToArray();

        AddMediatR(services, assemblies);
    }
}
