using Microsoft.EntityFrameworkCore;
using MobileMallDB.BusinessInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MobileMallDB.BusinessService
{
    public class BaseService : IBaseService
    {
        /// <summary>
        /// 使用protected 使继承的类也可以使用
        /// </summary>
        protected EFDBContext Context { get; set; }

        /// <summary>
        /// 返回 DBContext
        /// </summary>
        /// <param name="context"></param>
        public BaseService(EFDBContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// 根据id查询实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public EFDBContext GetContext()
        {
            return Context;
        }

        #region Query
        /// <summary>
        /// 根据id查询实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> Find<T>(int id) where T : class
        {
            return await Context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// 提供对单表的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<List<T>> Set<T>() where T : class
        {
            return await Context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// 查询,这才是合理的做法，上端给条件，这里查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        public async Task<List<T>> Query<T>(Expression<Func<T, bool>> funcWhere) where T : class
        {
            return await Context.Set<T>().Where(funcWhere).ToListAsync();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="funcWhere"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="funcOrderby"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<PagingData<T>> QueryPage<T, S>(Expression<Func<T, bool>> funcWhere, int pageSize, int pageIndex, Expression<Func<T, S>> funcOrderby, bool isAsc = true) where T : class
        {
            var list = await Set<T>();
            if (funcWhere != null)
                list = await list.AsQueryable().Where(funcWhere).ToListAsync();
            if (isAsc)
                list = await list.AsQueryable().OrderBy(funcOrderby).ToListAsync();
            else
                list = await list.AsQueryable().OrderByDescending(funcOrderby).ToListAsync();
            PagingData<T> result = new PagingData<T>()
            {
                DataList = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                RecordCount = list.Count()
            };
            return result;
        }
        #endregion

        #region Add
        /// <summary>
        /// 新增数据，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> Insert<T>(T t) where T : class
        {
            await Context.Set<T>().AddAsync(t);
            return await Commit();
        }

        /// <summary>
        /// 新增数据，及时Commit 多条sql 一个连接 事务插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        /// <returns></returns>
        public async Task<bool> Insert<T>(List<T> tList) where T : class
        {
            await Context.Set<T>().AddRangeAsync(tList);
            return await Commit();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新数据，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public async Task<bool> Update<T>(T t) where T : class
        {
            if (t == null) throw new Exception("t is null");
            Context.Set<T>().Attach(t); // 将数据附加到上下文，支持实体修改和新实体，重置为UnChanged
            Context.Entry(t).State = EntityState.Modified;
            return await Commit();
        }

        /// <summary>
        /// 更新数据，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        /// <returns></returns>
        public async Task<bool> Update<T>(List<T> tList) where T : class
        {
            Context.Set<T>().AttachRange(tList); // 将数据附加到上下文，支持实体修改和新实体，重置为UnChanged
            Context.Entry(tList).State = EntityState.Modified;
            return await Commit();
        }
        #endregion

        #region Delete
        /// <summary>
        /// 根据主键删除数据，及时Commit  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public async Task<bool> Delete<T>(int id) where T : class
        {
            T t = await Find<T>(id);
            if (t == null) throw new Exception("t is null");
            Context.Set<T>().Remove(t);
            return await Commit();
        }

        /// <summary>
        /// 删除数据，及时Commit  先附加 再删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public async Task<bool> Delete<T>(T t) where T : class
        {
            if (t == null) throw new Exception("t is null");
            Context.Set<T>().Attach(t);
            Context.Set<T>().Remove(t);
            return await Commit();
        }

        /// <summary>
        ///  删除数据集合，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        public async Task<bool> Delete<T>(List<T> tList) where T : class
        {
            Context.Set<T>().AttachRange(tList);
            Context.Set<T>().RemoveRange(tList);
            return await Commit();
        }
        #endregion

        #region Other
        /// <summary>
        /// 立即保存全部修改
        /// </summary>
        public async Task<bool> Commit()
        {
            return await Context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 执行sql 返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<T> ExcuteQuery<T>(string sql) where T : class
        {
            var products = Context.Set<T>().FromSqlRaw(sql).ToList();
            return products;
        }

        /// <summary>
        /// 执行sql集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        public async Task<bool> ExecuteSql(List<string> sqlArray)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var sql in sqlArray)
                    {
                        int InsertRusult = await Context.Database.ExecuteSqlRawAsync(sql);
                        if (InsertRusult < 1)
                        {
                            // 如果更新返回结果行小于一行，执行回滚
                            transaction.Rollback();
                            return false;
                        }
                    }
                    // 提交事务确保所有更改被保存
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // 出现异常，回滚事务
                    transaction.Rollback();
                    // 处理异常
                    throw new Exception("执行sql过程出现异常！" + ex.Message);
                }
                return true;
            }
        }

        public virtual async Task Dispose()
        {
            if (Context != null) await Context.DisposeAsync();
        }
        #endregion
    }
}
