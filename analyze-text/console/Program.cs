using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Azure;
using Azure.AI.TextAnalytics;

public class Program
{
    public static void Main(string[] args)
    {
        try
            {
                // Analyze each text file in the reviews folder
                var folderPath = Path.GetFullPath("./reviews");  
                DirectoryInfo folder = new DirectoryInfo(folderPath);

                
                // Create client using endpoint and key
                string aiSvcEndpoint = "YOUR_AZURE_OPEN_AI_ENDPOINT";
                string aiSvcKey = "YOUR_AZURE_OPEN_AI_KEY";
                
                AzureKeyCredential credentials = new AzureKeyCredential(aiSvcKey);
                Uri endpoint = new Uri(aiSvcEndpoint);
                TextAnalyticsClient aiClient = new TextAnalyticsClient(endpoint, credentials);

                
                foreach (var file in folder.GetFiles("*.txt"))
                {
                    // Read the file contents
                    #region read files
                    Console.WriteLine("\n-------------\n" + file.Name);
                    StreamReader sr = file.OpenText();
                    var text = sr.ReadToEnd();
                    sr.Close();
                    Console.WriteLine("\n" + text);
                    #endregion
                    // // Get language
                    DetectedLanguage detectedLanguage = aiClient.DetectLanguage(text);
                    Console.WriteLine($"\nLanguage: {detectedLanguage.Name}");
                    
                    // // Get sentiment
                    DocumentSentiment sentimentAnalysis = aiClient.AnalyzeSentiment(text);
                    Console.WriteLine($"\nSentiment: {sentimentAnalysis.Sentiment}");
                    
                    // Get key phrases
                    KeyPhraseCollection phrases = aiClient.ExtractKeyPhrases(text);
                    if (phrases.Count > 0)
                    {
                        Console.WriteLine("\nKey Phrases:");
                        foreach(string phrase in phrases)
                        {
                            Console.WriteLine($"\t{phrase}");
                        }
                    }
                    // Get entities
                    CategorizedEntityCollection entities = aiClient.RecognizeEntities(text);
                    if (entities.Count > 0)
                    {
                        Console.WriteLine("\nEntities:");
                        foreach(CategorizedEntity entity in entities)
                        {
                            Console.WriteLine($"\t{entity.Text} ({entity.Category})");
                        }
                    }

                    // Get linked entities
                    LinkedEntityCollection linkedEntities = aiClient.RecognizeLinkedEntities(text);
                    if (linkedEntities.Count > 0)
                    {
                        Console.WriteLine("\nLinks:");
                        foreach(LinkedEntity linkedEntity in linkedEntities)
                        {
                            Console.WriteLine($"\t{linkedEntity.Name} ({linkedEntity.Url})");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
    }
}