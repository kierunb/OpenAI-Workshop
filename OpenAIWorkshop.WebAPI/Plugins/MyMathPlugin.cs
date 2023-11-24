using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace OpenAIWorkshop.WebAPI.Plugins
{
    public class MathPlugin
    {
        [SKFunction, Description("Take the square root of a number")]
        public double Sqrt([Description("The number to take a square root of")] double input)
        {
            return System.Math.Sqrt(input);
        }
    }
}
