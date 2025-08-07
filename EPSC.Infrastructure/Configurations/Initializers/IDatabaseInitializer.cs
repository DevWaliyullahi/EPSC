namespace EPSC.Infrastructure.Configurations.Initializers
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }
}
