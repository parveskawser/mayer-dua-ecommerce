using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDUA.Framework
{
    public class ClientContext : IDisposable
    {
        private Client _Client;
        private readonly ResourcePool _Pool = new ResourcePool();

        public ClientContext(Client client)
        {
            _Client = client;
        }

        public Client TheClient
        {
            get
            {
                return _Client;
            }
        }

        /// <summary>
        /// Index property for storing items in the resource pool
        /// </summary>
        public IDisposable this[object key]
        {
            get
            {
                if (Check(key))
                    return _Pool.GetResource(key);
                else
                    return null;
            }
            set
            {
                _Pool.SetResource(key, value);
            }
        }

        public IDisposable this[Type type]
        {
            get
            {
                string key = type.FullName;
                IDisposable obj = _Pool.GetResource(key);
                if (obj == null)
                {
                    try
                    {
                        obj = (IDisposable)Activator.CreateInstance(type, new object[] { this });

                        _Pool.SetResource(key, obj);
                        return obj;
                    }
                    catch (Exception ex)
                    {
                        //Write log for error.
                        return null;
                    }
                }
                else
                    return _Pool.GetResource(key);
            }
        }

        public bool Check(object key)
        {
            return _Pool.CheckResource(key);
        }

        /// <summary>
        /// Gets resource from 2 level resource pool.
        /// </summary>
        /// <param name="container">Name of container resource pool</param>
        /// <param name="item">Name of the actual resource inside the container</param>
        /// <returns>The object requested</returns>
        public IDisposable Get(object container, object item)
        {
            if (_Pool.CheckResource(container))
            {
                ResourcePool containerPool = (ResourcePool)_Pool.GetResource(container);
                return containerPool.GetResource(item);
            }
            else
                return null;
        }

        /// <summary>
        /// Stores item in the 2 level resource pool
        /// </summary>
        /// <param name="container">Name of the container resource pool</param>
        /// <param name="item">Name of the item</param>
        /// <param name="resource">The resource to store</param>
        public void Set(object container, object item, IDisposable resource)
        {
            if (_Pool.CheckResource(container))
            {
                ResourcePool containerPool = (ResourcePool)_Pool.GetResource(container);
                containerPool.SetResource(item, resource);
            }
            else
            {
                ResourcePool containerPool = new ResourcePool();
                _Pool.SetResource(container, containerPool);
                containerPool.SetResource(item, resource);
            }
        }

        /// <summary>
        /// Removes item from resource pool
        /// </summary>
        /// <param name="item">key of the item to remove</param>
        public void Remove(object item)
        {
            _Pool.Remove(item);
        }

        public void Dispose()
        {
            // Dispose client
            if (_Client != null)
            {
                _Client.Dispose();
            }
            // Release client
            _Client = null;

            // Dispose resource pool
            _Pool.Dispose();
        }
    }
}