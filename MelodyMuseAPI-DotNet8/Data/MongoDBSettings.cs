namespace MelodyMuseAPI_DotNet8.Data
{
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CertificatePath { get; set; } = null!;
    }
}
