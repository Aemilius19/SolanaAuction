using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.DTO_s
{
    public class ChangeUsernameDto
    {
        public string WalletAddress { get; set; }
        
        public string NewUsername { get; set; }
    }
}
