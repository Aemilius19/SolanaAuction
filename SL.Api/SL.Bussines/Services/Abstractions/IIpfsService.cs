using SL.Core.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Bussines.Services.Abstractions
{
    public interface IIpfsService
    {
        Task<string> UploadMetadataAsync(NftMetadata metadata);
    }
}
