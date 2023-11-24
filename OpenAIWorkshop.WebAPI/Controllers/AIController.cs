using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Planners;
using System.Text.Json;

namespace OpenAIWorkshop.WebAPI.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IKernel _kernel;

        public AIController(IKernel kernel)
        {
            _kernel = kernel;
        }

        [HttpGet("semantic-function")]
        public async Task<IActionResult> FirstScenario(string userQuestion)
        {
            // scenario number 1: semantic function
            string prompt = @"Bot: How can I help you?
            User: {{$input}}

            ---------------------------------------------

            The intent of the user in 5 words or less: ";

            OpenAIRequestSettings requestSettings = new()
            {
                ExtensionData = {
                    {"MaxTokens", 500},
                    {"Temperature", 0.0},
                    {"TopP", 0.0},
                    {"PresencePenalty", 0.0},
                    {"FrequencyPenalty", 0.0}
                }
            };
            var getShortDescriptionFunction =
                _kernel.CreateSemanticFunction(prompt, requestSettings, functionName: "GetShortDescription");

            //string userQuestion = "I want to buy a new car";
            var result = await _kernel.RunAsync(userQuestion, getShortDescriptionFunction);

            return Ok(result.ToString());
        }

        [HttpGet("native-function")]
        public async Task<IActionResult> NativeFunction(int number)
        {
            var mathPlugin = _kernel.ImportFunctions(new Plugins.MathPlugin(), "MathPlugin");

            var result = await _kernel.RunAsync(number.ToString(), mathPlugin["Sqrt"]);

            return Ok(result.ToString());
        }


        [HttpGet("planner")]
        public async Task<IActionResult> Planner(string ask)
        {
            var mathPlugin = _kernel.ImportFunctions(new Plugins.MathPlugin(), "MathPlugin");

            // Create a planner
            var planner = new SequentialPlanner(_kernel);

            //var ask = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?";
            //var ask = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?";
            var plan = await planner.CreatePlanAsync(ask);

            Console.WriteLine("\nPlan:\n");
            Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));

            var result = await _kernel.RunAsync(plan);

            return Ok(result.ToString());
        }

        [HttpGet("chat-rag")]
        public async Task<IActionResult> ChatRag(string ask)
        {
            var memory = new KernelMemoryBuilder()
            //.WithOpenAIDefaults(GetApiKey())

            .WithAzureCognitiveSearch("https://cognitive-search-bk.search.windows.net", "")
            //.WithAzureFormRecognizer // TODO
            .Build<MemoryServerless>();


            string prompt = 
                @"Bot: Hello, I am a bot. I am here to help you with your finances.
                Please use details provided below:
                {{$memory}}   
                
                Provide an answer to the question.
                ";

            OpenAIRequestSettings requestSettings = new()
            {
                ExtensionData = {
                    {"MaxTokens", 500},
                    {"Temperature", 0.0},
                    {"TopP", 0.0},
                    {"PresencePenalty", 0.0},
                    {"FrequencyPenalty", 0.0}
                }
            };

            // we need to fetch data from memory
           

            var context = _kernel.CreateNewContext();
            
            var data = await memory.SearchAsync(ask, index: "regulamin-1", limit: 3, minRelevance: 0.80);

            string dataString = String.Empty;
            data.Results.ForEach(r =>
            {
                dataString += r.ToString(); 
            });

            context.Variables.Add("memory", dataString);

            var chatFunction =
                _kernel.CreateSemanticFunction(prompt, requestSettings, functionName: "ChatRag");

            //var response = await _kernel.RunAsync(context, chatFunction);


            return Ok();
        }

    }





        
}
