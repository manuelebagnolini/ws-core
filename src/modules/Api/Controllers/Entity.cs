﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using core.Extensions.Data;

namespace core.Api.Controllers
{
    [Route("api/[controller]")]
    public class EntityController<T> : ControllerBase where T : IEntity
    {   
        protected IRepository<T> _repository;

        public EntityController(IRepository<T> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public virtual IActionResult Get()
        {
            return Ok(_repository.List);
        }

        [HttpGet("{id}")]
        public virtual IActionResult Get(string id)
        {
            return Ok(_repository.Find(id));
        }

        [HttpPost]
        public virtual void Post([FromBody]T entity)
        {
            _repository.Add(entity);
        }

        [HttpPut("{id}")]
        public virtual void Put(string id, [FromBody]T entity)
        {

            _repository.Update(entity);
        }

        [HttpDelete("{id}")]
        public virtual void Delete([FromBody]T entity)
        {
            _repository.Delete(entity);
        }
    }
}
