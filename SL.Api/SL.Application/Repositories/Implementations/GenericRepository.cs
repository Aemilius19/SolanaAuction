using Microsoft.EntityFrameworkCore;
using SL.Application.Data.Context;
using SL.Application.Repositories.Abstractions;
using SL.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Application.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly SlDbContext _context;

        public GenericRepository(SlDbContext context)
        {
            _context = context;
        }

        public async Task<T> Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteById(string id)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public IQueryable<T> GetAll(params string[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

        public async Task<T> GetById(string id)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            return entity ?? throw new Exception("Entity not found.");
        }

        public async Task<T> Update(T entity)
        {
            var existingEntity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == entity.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            throw new Exception("Entity not found.");
        }
    }
}
