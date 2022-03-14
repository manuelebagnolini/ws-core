﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data
{

    public interface IEntity
    {  
    }

    public interface IEntity<TKey> :IEntity where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
        //void OnChange(EntityChangeEventContext.ActionTypes action);        
    }

    public class Entity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }
        
        /*
        public void OnChange(EntityChangeEventContext.ActionTypes action)
        {              

         
                         var ctx = new EntityChangeEventContext()
            {
                Entity = this,
                Action = action
            }; 
            try
            {
                ExtCore.Events.Event<IEntityChangeEvent, EntityChangeEventContext>.Broadcast(ctx);

                ExtCore.Events.Event<IEntityChangeEvent<TKey>, EntityChangeEventContext>.Broadcast(ctx);

                ExtCore.Events.Event<IEntityChangeEvent<IEntity<TKey>, TKey>, EntityChangeEventContext>.Broadcast(ctx);
            }
            catch { }         

        }
        */

    }

    /*
    #region "Entity scalar-value model helper"
    public class EntityOfInt : Ws.Core.Extensions.Data.Entity<int> { }
    public class EntityOfLong : Ws.Core.Extensions.Data.Entity<long> { }
    public class EntityOfString : Ws.Core.Extensions.Data.Entity<string> { }
    public class EntityOfGuid : Ws.Core.Extensions.Data.Entity<Guid> { }
    #endregion
    */

}