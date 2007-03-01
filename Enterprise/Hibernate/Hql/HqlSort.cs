using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    /// <summary>
    /// Provides support for building HQL queries dynamically from <see cref="SearchCriteria"/> objects.
    /// </summary>
    /// <seealso cref="HqlQuery"/>
    public class HqlSort : HqlElement, IComparable<HqlSort>
    {
        /// <summary>
        /// Extracts a list of <see cref="HqlSort"/> objects from the specified <see cref="SearchCriteria"/>
        /// </summary>
        /// <param name="alias">The HQL alias to prepend to the sort variables</param>
        /// <param name="criteria">The search criteria object</param>
        /// <returns>A list of HQL sort object that are equivalent to the specified criteria</returns>
        public static HqlSort[] FromSearchCriteria(string alias, SearchCriteria criteria)
        {
            List<HqlSort> hqlSorts = new List<HqlSort>();
            foreach (SearchCriteria subCriteria in criteria.SubCriteria.Values)
            {
                if (subCriteria is SearchConditionBase)
                {
                    SearchConditionBase sc = (SearchConditionBase)subCriteria;
                    if (sc.SortPosition > -1)
                    {
                        hqlSorts.Add(new HqlSort(alias, sc.GetKey(), sc.SortDirection, sc.SortPosition));
                    }
                }
                else
                {
                    // recur on subCriteria
                    string subAlias = string.Format("{0}.{1}", alias, subCriteria.GetKey());
                    hqlSorts.AddRange(FromSearchCriteria(subAlias, subCriteria));
                }
            }
            return hqlSorts.ToArray();
        }

        private string _hql;
        private int _position;

        /// <summary>
        /// Constructs an <see cref="HqlSort"/> object.
        /// </summary>
        /// <param name="alias">The HQL alias to prepend</param>
        /// <param name="fieldName">The HQL field name</param>
        /// <param name="ascending">Specifies whether the sort is ascending or descending</param>
        /// <param name="position">Specifies the relative priority of the sort condition</param>
        public HqlSort(string alias, string fieldName, bool ascending, int position)
        {
            _hql = string.Format("{0}.{1} {2}", alias, fieldName, ascending ? "asc" : "desc");
            _position = position;
        }

        /// <summary>
        /// The HQL for this sort.
        /// </summary>
        public override string Hql
        {
            get { return _hql; }
        }

        /// <summary>
        /// The position of this sort in the order by clause.
        /// </summary>
        public int Position
        {
            get { return _position; }
        }

        #region IComparable<HqlSort> Members

        public int CompareTo(HqlSort other)
        {
            return this._position.CompareTo(other._position);
        }

        #endregion
    }
}
