namespace ASPNet_Authentication.Database
{
    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }

        string CollectionName { get; set; }
        string DatabaseName { get; set; }
    }
}