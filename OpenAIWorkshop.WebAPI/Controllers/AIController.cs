using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

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

    }

        
}
