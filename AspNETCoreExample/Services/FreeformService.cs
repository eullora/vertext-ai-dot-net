using Google.Cloud.AIPlatform.V1;
using static Google.Cloud.AIPlatform.V1.SafetySetting.Types;
using static Google.Cloud.AIPlatform.V1.PredictionServiceClient;

namespace TechStrat.RGPT.UI.Services
{
    public interface IFreeformService
    {
        StreamGenerateContentStream StreamGenerateContent(string query, string groundingSource ="");        
    }
    public class FreeformService : IFreeformService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _rootPath;
        private readonly string _publisher;
        private readonly string _credentialsPath;        
        private readonly string _instructions;
        private readonly string _locaion;
        private readonly string _projectId;
        private readonly string _modelName;
        private string _dataStore;
        private string _query;

        public FreeformService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _rootPath = _webHostEnvironment.ContentRootPath;
            _publisher = "google";
            _locaion = "your-project-location";
            _projectId = "your-project-id";
            _modelName = "";
            _credentialsPath = "credentials.json";
            
            _instructions = @"You are a front-end web developer specializing in creating responsive websites using Bootstrap 5 and FontAwesome.  Your task is to generate HTML code from an image of a website layout. The user will provide the image.
                **Instructions:**

                1. Analyze the provided image of the website layout
                2. Identify the different elements within the layout (e.g., header, navigation, sections, footer).
                3. Generate clean, well-commented HTML code that accurately reflects the layout in the image. This layout will need to respect text dimensions, paddings and alignments.
                4. Use Bootstrap 5 for responsiveness and styling.
                5. Incorporate FontAwesome icons where appropriate based on the image.
                6. Ensure the generated HTML is valid and follows best practices for web development.
                7. If the image is unclear or you are unable to interpret specific elements, add comments in the HTML explaining the uncertainties and your assumptions.  For example, if a button's functionality is unclear, comment with `<!-- Button functionality unclear. Assumed to be a submit button. -->`.
                8. If the image is too complex or contains elements beyond the scope of HTML and CSS, indicate this in a comment at the beginning of the code and focus on generating the core structure and layout.

                **Output:**

                Provide the complete HTML code.  Ensure the code is properly indented and commented for readability.";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="groundingSource">
        /// e.g. projects/{projectname}/locations/global/collections/default_collection/dataStores/{my-datastore-1880821220306}
        /// </param>
        /// <returns></returns>
        public StreamGenerateContentStream StreamGenerateContent(string query, string groundingSource = "")
        {
            _query = query;
            _dataStore = groundingSource;

            var serviceClient = ConstructServiceClient();

            var contentRequest = !string.IsNullOrEmpty(_dataStore) ? GenerateWithGrounding() : GenerateWithoutGrounding();
            return serviceClient.StreamGenerateContent(contentRequest);
        }      
        private PredictionServiceClient ConstructServiceClient()
        {
            return new PredictionServiceClientBuilder
            {
                Endpoint = $"{_locaion}-aiplatform.googleapis.com",
                CredentialsPath = Path.Combine(_rootPath, _credentialsPath)
            }.Build();
        }
        private GenerateContentRequest GenerateWithGrounding()
        {             
            var request = new GenerateContentRequest
            {
                Model = $"projects/{_projectId}/locations/{_locaion}/publishers/{_publisher}/models/{_modelName}",
                SafetySettings =
                {
                    new SafetySetting
                    {
                        Category = HarmCategory.HateSpeech,
                        Threshold = HarmBlockThreshold.Off
                    },
                    new SafetySetting
                    {
                        Category= HarmCategory.DangerousContent,
                        Threshold = HarmBlockThreshold.Off
                    },
                    new SafetySetting
                    {
                        Category = HarmCategory.SexuallyExplicit,
                        Threshold = HarmBlockThreshold.Off
                    },
                    new SafetySetting
                    {
                        Category = HarmCategory.Harassment,
                        Threshold = HarmBlockThreshold.Off
                    }
                },

                SystemInstruction = new Content
                {
                    Parts =
                    {
                        new Part {
                            Text = File.ReadAllText(Path.Combine(_rootPath,"Prompts", _instructions))
                        }
                    }
                },

                GenerationConfig = new GenerationConfig
                {
                    Temperature = 1,
                    TopP = 0.95f,
                    MaxOutputTokens = 8192,
                },

                Tools =
                {
                    new Tool
                    {
                        Retrieval = new Retrieval
                        {
                            VertexAiSearch = new VertexAISearch
                            {
                                Datastore = _dataStore
                            }
                        }
                    }
                }
            };

            request.Contents.Add(new Content
            {
                Role = "USER",
                Parts =
                {
                    new Part { Text = _query }
                }
            });

            return request; 
        }
        private GenerateContentRequest GenerateWithoutGrounding()
        {

            var request =  new GenerateContentRequest
            {
                Model = $"projects/{_projectId}/locations/{_locaion}/publishers/{_publisher}/models/{_modelName}",
                SafetySettings =
                {
                    new SafetySetting
                    {
                        Category = HarmCategory.HateSpeech,
                        Threshold = HarmBlockThreshold.Off
                    },
                    new SafetySetting
                    {
                        Category= HarmCategory.DangerousContent,
                        Threshold = HarmBlockThreshold.Off
                    },
                    new SafetySetting
                    {
                        Category = HarmCategory.SexuallyExplicit,
                        Threshold = HarmBlockThreshold.Off
                    },
                    new SafetySetting
                    {
                        Category = HarmCategory.Harassment,
                        Threshold = HarmBlockThreshold.Off
                    }
                },

                SystemInstruction = new Content
                {
                    Parts =
                    {
                        new Part {
                            Text = File.ReadAllText(Path.Combine(_rootPath,"Prompts", _instructions))
                        }
                    }
                },

                GenerationConfig = new GenerationConfig
                {
                    Temperature = 1,
                    TopP = 0.95f,
                    MaxOutputTokens = 8192
                }
            };

            var content = new Content
            {
                Role = "USER",
                Parts =
                {
                    new Part { Text = _query }
                }
            };
            
            request.Contents.Add(content);
            return request;
        }          
    }
}
