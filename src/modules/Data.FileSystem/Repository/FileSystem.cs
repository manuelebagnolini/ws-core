﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ws.Core.Extensions.Data.Repository
{
    public class FileSystem<T, TKey> : IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private List<T> _collection = new List<T>();
        private static Data.FileSystem.Options _options { get; set; } = new Data.FileSystem.Extension()._options ?? new Data.FileSystem.Options();        
        private string _path { get; set; }

        public FileSystem(IWebHostEnvironment env, ILoggerFactory logger)
        {            
            _path = System.IO.Path.Combine(env.ContentRootPath, _options.Folder, $"{typeof(T).Name}.json");

            using (var stream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                var readed = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(readed))
                    _collection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(readed);
                else
                    logger.CreateLogger("Data.Repository.Logger").LogWarning($"Path {_path} not found");
            }
        }


        IQueryable<T> IRepository<T, TKey>.List => _collection.AsQueryable();

        public T Find(TKey Id)
        { 
            return _collection.FirstOrDefault<T>(_ => _.Id.Equals(Id));
        }

        public IQueryable<T> Query(FormattableString command)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            if (entity != null)
            {
                _collection.Add(entity);
                Save();
                //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Create);
            }
        }

        public void AddMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _collection.AddRange(entities);
                Save();
            }
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                var item = Find(entity.Id);
                if (item != null)
                    _collection[_collection.IndexOf(item)] = entity;
                Save();
                //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Update);            
            }
        }
        public void UpdateMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _collection
                .Join(entities, o => o.Id, i => i.Id, (o, i) => (o, i))
                .AsParallel()
                .ForAll(_ => _collection[_collection.IndexOf(_.o)] = _.i);
                Save();
            }
        }

        public void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (entities != null && entities.Any())
            {
                switch (operation)
                {
                    case RepositoryMergeOperation.Upsert:
                        _collection = entities.Union(_collection, new EntityComparer<T, TKey>()).ToList();
                        break;
                    case RepositoryMergeOperation.Sync:
                        _collection = entities.ToList();
                        break;
                }
                Save();
            }
        }

        public void Delete(T entity)
        {
            if (entity != null)
            {
                _collection.RemoveAll(_ => _.Id.Equals(entity.Id));
                Save();
                //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Delete);
            }
        }

        public void DeleteMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _collection.RemoveAll(_ => entities.Any(__ => __.Id.Equals(_.Id)));
                Save();
            }
        }

        private void Save()
        {

            var jsonSetting = _options?.Serialization?.ToJsonSerializerSettings();
            File.WriteAllText(_path, Newtonsoft.Json.JsonConvert.SerializeObject(_collection, jsonSetting));
        }
    }
}
