using Microsoft.AspNetCore.Identity;
using SL.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Application.Repositories.Abstractions
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> Add(T entity);
        Task<bool> DeleteById(string id);
        IQueryable<T> GetAll(params string[] includes);
        Task<T> GetById(string id);
        Task<T> Update(T entity);
    }
}
