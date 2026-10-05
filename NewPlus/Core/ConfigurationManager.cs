using NewPlus.Models;
using System.Text.Json;



namespace NewPlus.Core
{
    public static class ConfigurationManager
    {
        #region Configuration
        private static readonly string _standardUserDocumentsPath_A = $@"C:\Users\{Environment.UserName}\Documents";
        private static readonly string _standardUserDocumentsPath_B = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string StandardUserDocumentsPath { get; set; }

        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NewPlus");
        public static readonly string IcoPath = Path.Combine(AppDataPath, "ICO");
        public static readonly string TemplatesPath = Path.Combine(AppDataPath, "Templates");

        public static readonly string Json_TemplatesConfigPath = Path.Combine(AppDataPath, "templates.json");
        public static readonly string Json_ExtensionsConfigPath = Path.Combine(AppDataPath, "extensions.json");

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        #endregion



        #region PublicMethods
        public static void InitializeEnvironment()
        {
            if (Directory.Exists(_standardUserDocumentsPath_A))
            {
                StandardUserDocumentsPath = _standardUserDocumentsPath_A;
            }
            else if (Directory.Exists(_standardUserDocumentsPath_B))
            {
                StandardUserDocumentsPath = _standardUserDocumentsPath_B;
            }

            Directory.CreateDirectory(AppDataPath);
            Directory.CreateDirectory(IcoPath);
            Directory.CreateDirectory(TemplatesPath);

            if (!File.Exists(Json_TemplatesConfigPath))
            {
                SaveTemplates(new List<TemplateProfile>());
            }
        }

        public static List<TemplateProfile> LoadTemplates()
        {
            if (!File.Exists(Json_TemplatesConfigPath))
            {
                return new List<TemplateProfile>();
            }

            try
            {
                string json = File.ReadAllText(Json_TemplatesConfigPath);
                return JsonSerializer.Deserialize<List<TemplateProfile>>(json, JsonOptions) ?? new List<TemplateProfile>();
            }
            catch (JsonException)
            {
                return new List<TemplateProfile>();
            }
        }

        public static List<ExtensionProfile> LoadExtensions()
        {
            if (!File.Exists(Json_ExtensionsConfigPath))
            {
                // Write an empty array to establish the file without hardcoding data
                File.WriteAllText(Json_ExtensionsConfigPath, "[\n]");
                return new List<ExtensionProfile>();
            }

            try
            {
                string json = File.ReadAllText(Json_ExtensionsConfigPath);
                return JsonSerializer.Deserialize<List<ExtensionProfile>>(json, JsonOptions) ?? new List<ExtensionProfile>();
            }
            catch (JsonException)
            {
                return new List<ExtensionProfile>();
            }
        }

        public static void SaveTemplates(List<TemplateProfile> templates)
        {
            string json = JsonSerializer.Serialize(templates, JsonOptions);
            File.WriteAllText(Json_TemplatesConfigPath, json);
        }
        #endregion
    }
}