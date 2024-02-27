namespace MelodyMuseAPI_DotNet8.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CertificatePath { get; set; } = null!;
    }
}
