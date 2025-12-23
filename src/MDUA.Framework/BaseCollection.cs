using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace MDUA.Framework
{
    /// <summary>
    /// class BaseCollection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [CollectionDataContract]
    public class BaseCollection<T> : List<T>, IDisposable
    {
        private BaseBusinessEntity.RowStateEnum _RowStateParam;
        
        /// <summary>
        /// method: MatchRowState
        /// checks for matching row states
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool MatchRowState(T obj)
        {
            BaseBusinessEntity baseObj = obj as BaseBusinessEntity;

            if (obj != null)
                return baseObj.RowState == _RowStateParam;

            return false;
        }

        /// <summary>
        /// method FindByRowState
        /// returns a list of objects of type T with row state equal to 'state'
        /// </summary>
        /// <param name="state">any value from BaseBusinessEntity.RowStateEnum</param>
        /// <returns></returns>
        List<T> FindByRowState(BaseBusinessEntity.RowStateEnum state)
        {
            _RowStateParam = state;
            return FindAll(MatchRowState);
        }

        /// <summary>
        /// method FindAllNewRow
        /// returns all the rows that have a row state of 'NewRow'
        /// </summary>
        /// <returns></returns>
        List<T> FindAllNewRow()
        {
            return FindByRowState(BaseBusinessEntity.RowStateEnum.NewRow);
        }

        /// <summary>
        /// method FindAllUpdatedRow
        /// returns all the objects with row state 'UpdatedRow'
        /// </summary>
        /// <returns></returns>
        List<T> FindAllUpdatedRow()
        {
            return FindByRowState(BaseBusinessEntity.RowStateEnum.UpdatedRow);
        }

        /// <summary>
        /// method FindAllDeletedRow
        /// returns all the the objects with row state 'DeletedRow' 
        /// </summary>
        /// <returns></returns>
        List<T> FindAllDeletedRow()
        {
            return FindByRowState(BaseBusinessEntity.RowStateEnum.DeletedRow);
        }

        /// <summary>
        /// virtual method Dispose
        /// member of IDisposable
        /// </summary>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// the default constructor
        /// </summary>
        public BaseCollection() { }

        /// <summary>
        /// Constructor for class BaseCollection
        /// </summary>
        /// <param name="list">an array of type T</param>
        public  BaseCollection(T[] list)
        {
            this.AddRange(list);
        }

        /// <summary>
        /// constructor for BaseCollection
        /// </summary>
        /// <param name="list">a list of type T</param>
        public  BaseCollection(List<T> list)
        {
            this.AddRange(list);
        }

    }
}
