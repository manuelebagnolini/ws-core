﻿using System;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions.Data.Cache;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Ws.Core.Extensions.Api.Controllers
{
    public class EntityCachedController<T, TKey> : EntityController<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        protected ICacheRepository<T, TKey> _cachedRepository;

        public EntityCachedController(IRepository<T, TKey> repository, ICacheRepository<T, TKey> cached) : base(repository)
        {
            _cachedRepository = cached;
        }
    }

    [Route("api/cache/[controller]")]
    public class EntityCachedControllerWithMethods<T, TKey> : EntityCachedController<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public EntityCachedControllerWithMethods(IRepository<T, TKey> repository, ICacheRepository<T, TKey> cached) : base(repository, cached) { }

        [HttpGet]
        public virtual IActionResult Get()
        {
            return Ok(_cachedRepository.List);
        }

        [HttpGet("{id}")]
        public virtual IActionResult GetById(TKey id)
        {
            return Ok(_cachedRepository.Find(id));
        }

        [HttpPost]
        public virtual void Post([FromBody]T entity)
        {
            _cachedRepository.Add(entity);
            //base.Post(entity);
        }

        [Route("many")]
        [HttpPost]
        public virtual void PostMany([FromBody]IEnumerable<T> entities)
        {
            _cachedRepository.AddMany(entities);
        }

        [HttpPut("{id}")]
        public virtual void Put(TKey id, [FromBody]T entity)
        {
            _cachedRepository.Update(entity);
            //base.Put(id,entity);
        }

        [HttpPut]
        public virtual void PutMany([FromBody]IEnumerable<T> entities)
        {
            _cachedRepository.UpdateMany(entities);
        }

        [Route("merge/{operation}")]
        [HttpPost]
        public virtual void Merge(RepositoryMergeOperation operation,[FromBody]IEnumerable<T> entities)
        {
            _cachedRepository.Merge(entities,operation);
        }

        [HttpDelete("{id}")]
        public virtual void Delete([FromBody]T entity)
        {
            _cachedRepository.Delete(entity);
            //base.Delete(entity);
        }

        [HttpDelete]
        public virtual void DeleteMany([FromBody]IEnumerable<T> entities)
        {
            _cachedRepository.DeleteMany(entities);
        }
    }
}
