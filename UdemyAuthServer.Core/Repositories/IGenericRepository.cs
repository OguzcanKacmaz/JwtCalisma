﻿using System.Linq.Expressions;

namespace UdemyAuthServer.Core.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);
    Task AddAsync(TEntity entity);
    void Remove(TEntity entity);
    TEntity Update(TEntity entity);
}
