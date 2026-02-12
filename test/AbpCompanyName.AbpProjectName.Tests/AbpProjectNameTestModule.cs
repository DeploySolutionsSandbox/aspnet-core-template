using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using AbpCompanyName.AbpProjectName.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AbpCompanyName.AbpProjectName.Tests;

[DependsOn(
    typeof(AbpProjectNameApplicationModule),
    typeof(AbpProjectNameEntityFrameworkCoreModule),
    typeof(AbpTestBaseModule)
    )]
public class AbpProjectNameTestModule : AbpModule
{
    private readonly IServiceCollection _services;
    private ServiceProvider _serviceProvider;

    public AbpProjectNameTestModule()
    {
        _services = new ServiceCollection();
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; // EF Core InMemory DB does not support transactions.
        SetupInMemoryDb();
    }

    public override void Initialize()
    {
        _serviceProvider = _services.BuildServiceProvider();
        RegisterServices();
    }

    private void SetupInMemoryDb()
    {
        _services.AddEntityFrameworkInMemoryDatabase();

        var builder = new DbContextOptionsBuilder<AbpProjectNameDbContext>();
        builder.UseInMemoryDatabase("Test").UseInternalServiceProvider(_services.BuildServiceProvider());

        _services.AddSingleton(builder.Options);
    }

    private void RegisterServices()
    {
        // Register application services manually or via reflection if needed.
        var assembly = typeof(AbpProjectNameTestModule).GetAssembly();
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsClass && !type.IsAbstract)
            {
                foreach (var iface in type.GetInterfaces())
                {
                    _services.AddTransient(iface, type);
                }
            }
        }
    }
}