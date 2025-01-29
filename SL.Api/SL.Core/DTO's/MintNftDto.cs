using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.DTO_s
{
    public class MintNftDto
    {
        public string UserPublicKey { get; set; }
        public string NftPublicKey { get; set; }
        public string MintPublicKey { get; set; }
        public string TokenAccountPublicKey { get; set; }
        public string MetadataUrl { get; set; }
    }
}
