using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.Helper
{
    public class OpenAiSettings
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; } = "https://api.openai.com/v1/images/generations";
    }
}
