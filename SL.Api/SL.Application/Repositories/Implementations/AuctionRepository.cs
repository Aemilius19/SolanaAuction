using SL.Application.Data.Context;
using SL.Application.Repositories.Abstractions;
using SL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Application.Repositories.Implementations
{
    public class AuctionRepository : GenericRepository<Auction>, IAuctionRepository
    {
        public AuctionRepository(SlDbContext context) : base(context)
        {
        }
    }
}
