using System.Text.Json.Serialization;



namespace NewPlus.Models
{
    public class TemplateProfile
    {
        #region Properties
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("category")]
        public string Category { get; set; } = "Uncategorized";

        [JsonPropertyName("templateFolderName")]
        public string TemplateFolderName { get; set; } = string.Empty;

        [JsonPropertyName("iconFileName")]
        public string IconFileName { get; set; } = string.Empty;

        [JsonPropertyName("fileRenameExtension")]
        public string FileRenameExtension { get; set; } = string.Empty;
        #endregion
    }
}