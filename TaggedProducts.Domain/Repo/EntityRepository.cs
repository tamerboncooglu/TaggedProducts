using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using TaggedProducts.Domain.Entity;

namespace TaggedProducts.Domain.Repo
{
    public static class Extensions
    {
        /// <summary>
        /// An extention method for repositories. 
        /// We use this method for getting paginated data.
        /// We don't want repositories to get all the data.
        /// All the services GetAll methods uses this extention method and returns data with given size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedList<T> ToPaginatedList<T>(this IQueryable<T> query, int pageIndex, int pageSize) where T : BaseEntity
        {
            return new PagedList<T>(pageIndex, pageSize, query.Skip((pageIndex - 1) * pageSize).Take(pageSize));
        }
    }

    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly MongoDatabase _mongoDatabase;
        private readonly MongoCollection<TEntity> _collection;

        public EntityRepository()
        {
            _mongoDatabase = new MongoClient().GetServer().GetDatabase("ProductTags");
            _collection = _mongoDatabase.GetCollection<TEntity>(typeof(TEntity).Name, WriteConcern.Acknowledged);
        }

        public MongoCursor<TEntity> FindAll()
        {
            return _collection.FindAllAs<TEntity>();
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _collection.AsQueryable().Where(x => !x.IsDeleted);
        }

        public IQueryable<TEntity> AsOrderedQueryable()
        {
            if (!_collection.IndexExists("CreatedOn"))
            {
                _collection.CreateIndex("CreatedOn");
            }

            return AsQueryable().OrderByDescending(x => x.CreatedOn);
        }

        public TEntity GetSingle(string id)
        {
            return GetSingle(x => x.Id == ObjectId.Parse(id));
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> expression)
        {
            return AsQueryable().Where(expression).FirstOrDefault();
        }

        public WriteConcernResult Add(TEntity entity)
        {
            return _collection.Insert(entity);
        }

        public void AddBulk(IEnumerable<TEntity> entities)
        {
            _collection.InsertBatch(entities);
        }

        public WriteConcernResult Delete(TEntity entity)
        {
            return _collection.Update(Query<TEntity>.EQ(x => x.Id, entity.Id),
                                      Update<TEntity>.Set(x => x.DeletedOn, entity.DeletedOn)
                                                     .Set(x => x.DeletedBy, entity.DeletedBy)
                                                     .Set(x => x.IsDeleted, true));
        }

        public WriteConcernResult Update(IMongoQuery mongoQuery, IMongoUpdate mongoUpdate)
        {
            return _collection.Update(mongoQuery, mongoUpdate);
        }
    }
}