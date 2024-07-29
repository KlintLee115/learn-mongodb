namespace backend
{
    public static class Utilities
    {
        public static string GetEnvironmentVariable(string name)
        {
            var value = Environment.GetEnvironmentVariable(name) ?? throw new Exception($"Environment variable '{name}' is not set.");
            return value;
        }
    }
}
