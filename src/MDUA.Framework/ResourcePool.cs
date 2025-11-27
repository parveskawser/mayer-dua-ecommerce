using System;
using System.Collections;
namespace MDUA.Framework
{
	/// <summary>
	/// Summary description for ResourcePool.
	/// </summary>
	public class ResourcePool : IDisposable
	{
		private Hashtable _ResourcePool;
		private const int POOL_SIZE = 3;

		public IDisposable GetResource( object key )
		{
			if( _ResourcePool == null )
				return null;
			return _ResourcePool[ key ] as IDisposable;
		}

		public void Remove( object key )
		{
			if( CheckResource( key ) )
			{
				IDisposable item = GetResource( key );
				if( item != null )
					item.Dispose();

				
				_ResourcePool.Remove( key );

			}
			
		}

		public void SetResource( object key, IDisposable disposableResource )
		{
			if( _ResourcePool == null )
				_ResourcePool = new Hashtable( POOL_SIZE );

			_ResourcePool[ key ] = disposableResource;
		}
		public bool CheckResource( object key )
		{
			if( _ResourcePool == null )
				return false;

			return _ResourcePool.ContainsKey( key );
		}


		public void Dispose()
		{
			// Dispose all resources
			if( _ResourcePool != null )
			{
				foreach( IDisposable resource in _ResourcePool.Values )
				{
					resource.Dispose();
				}
				_ResourcePool.Clear();
			}
			
		}
	}
}
