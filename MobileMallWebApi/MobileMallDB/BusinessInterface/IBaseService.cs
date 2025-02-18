using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MobileMallDB.BusinessInterface
{
    public interface IBaseService
    {
        /// <summary>
        /// 返回 DBContext
        /// </summary>
        /// <param name="context"></param>
        EFDBContext GetContext();

        #region Query
        /// <summary>
        /// 根据id查询实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> Find<T>(int id) where T : class;

        /// <summary>
        /// 提供对单表的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<List<T>> Set<T>() where T : class;

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        Task<List<T>> Query<T>(Expression<Func<T, bool>> funcWhere) where T : class;

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
        Task<PagingData<T>> QueryPage<T, S>(Expression<Func<T, bool>> funcWhere, int pageSize, int pageIndex, Expression<Func<T, S>> funcOrderby, bool isAsc = true) where T : class;
        #endregion

        #region Add
        /// <summary>
        /// 新增数据，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> Insert<T>(T t) where T : class;

        /// <summary>
        /// 新增数据，及时Commit 多条sql 一个连接 事务插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        /// <returns></returns>
        Task<bool> Insert<T>(List<T> tList) where T : class;
        #endregion

        #region Update
        /// <summary>
        /// 更新数据，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        Task<bool> Update<T>(T t) where T : class;

        /// <summary>
        /// 更新数据，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        /// <returns></returns>
        Task<bool> Update<T>(List<T> tList) where T : class;
        #endregion

        #region Delete
        /// <summary>
        /// 根据主键删除数据，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        Task<bool> Delete<T>(int id) where T : class;

        /// <summary>
        /// 删除数据，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        Task<bool> Delete<T>(T t) where T : class;

        /// <summary>
        ///  删除数据集合，及时Commit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        Task<bool> Delete<T>(List<T> tList) where T : class;
        #endregion

        #region Other
        /// <summary>
        /// 立即保存全部修改
        /// </summary>
        Task<bool> Commit();

        /// <summary>
        /// 执行sql 返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        List<T> ExcuteQuery<T>(string sql) where T : class;

        /// <summary>
        /// 执行sql 无返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlArray"></param>
        Task<bool> ExecuteSql(List<string> sqlArray);
        #endregion
    }
}
