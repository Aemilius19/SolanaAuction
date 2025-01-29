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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SlDbContext context) : base(context)
        {
        }
    }
}
