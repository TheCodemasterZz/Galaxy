﻿using Galaxy.Domain;
using Galaxy.Infrastructure;
using Galaxy.ObjectMapping;
using Galaxy.Repositories;
using Galaxy.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Application
{
    public class CrudAppService<TEntity, TEntityDto, TKey> : QueryAppServiceAsync<TEntity, TEntityDto, TKey>, ICrudAppService, IApplicationService 
         where TEntity : class, IEntity<TKey>, IAggregateRoot, IObjectState
    {
        protected readonly IUnitOfWorkAsync UnitOfWorkAsync;
        public CrudAppService(IRepositoryAsync<TEntity, TKey> repositoryAsync
            , IUnitOfWorkAsync unitOfWorkAsync
            , IObjectMapper objectMapper) : base(repositoryAsync, objectMapper)
        {
            UnitOfWorkAsync = unitOfWorkAsync ?? throw new ArgumentNullException(nameof(unitOfWorkAsync));
        }

        [DisableUnitOfWork]
        public virtual TEntityDto Add(Func<TEntity> when)
        {
            var aggregate = when();

            var insertedAggregate = _repositoryAsync.Insert(aggregate);

            UnitOfWorkAsync.SaveChanges();

            return base._objectMapper.MapTo<TEntityDto>(
               insertedAggregate
             );
        }

        [DisableUnitOfWork]
        public virtual TEntityDto Update(TKey id, Action<TEntity> when)
        {
            TEntity aggregate =  base._repositoryAsync.Find(id);

            when(aggregate);

            aggregate =_repositoryAsync.Update(aggregate);

            UnitOfWorkAsync.SaveChanges();

            return base._objectMapper.MapTo<TEntityDto>(
             aggregate
             );
        }

        [DisableUnitOfWork]
        public virtual void Delete(TKey id)
        {
            _repositoryAsync.Delete(id);

            UnitOfWorkAsync.SaveChanges();
        }
    }
    
}

    


