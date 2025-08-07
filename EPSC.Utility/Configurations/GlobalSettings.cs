namespace EPSC.Utility.Configurations
{
    public class GlobalSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public JwtSettings JwtSettings { get; set; } = new JwtSettings();
    }
}
