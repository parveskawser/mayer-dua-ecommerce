using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Collections;

namespace MDUA.Framework
{
    [Serializable]
    public sealed class PagedRequest
    {
        private int _TotalRows;
        private int _pageIndex;
        private int _rowPerPage;
        private string _sortColumn;
        private string _sortOrder;
        private StringDictionary _conditions;
        private string _WhereClause = string.Empty;

        public int PageIndex
        {
            [DebuggerStepThrough()]
            get
            {
                return _pageIndex;
            }
            [DebuggerStepThrough()]
            set
            {
                _pageIndex = value;
            }
        }

        public int RowPerPage
        {
            [DebuggerStepThrough()]
            get
            {
                return _rowPerPage;
            }
            [DebuggerStepThrough()]
            set
            {
                _rowPerPage = value;
            }
        }

        public StringDictionary Conditions
        {
            [DebuggerStepThrough()]
            get
            {
                if (_conditions == null)
                {
                    _conditions = new StringDictionary();
                }

                return _conditions;
            }
        }

        public string SortColumn
        {
            [DebuggerStepThrough()]
            get
            {
                return _sortColumn;
            }
            [DebuggerStepThrough()]
            set
            {
                _sortColumn = value;
            }
        }

        public string SortOrder
        {
            [DebuggerStepThrough()]
            get
            {
                return _sortOrder;
            }
            [DebuggerStepThrough()]
            set
            {
                _sortOrder = value;
            }
        }

        public int TotalRows
        {  [DebuggerStepThrough()]
            get
            {
                return _TotalRows;
            }  [DebuggerStepThrough()]
            set
            {
                _TotalRows = value;
            }
        }

        private string Convert(object value)
        {
            if ((value == null) || (value == DBNull.Value))
            {
                return string.Empty;
            }
            else
            {
                return value.ToString().Trim();
            }
        }
        public String WhereClause
        {
            get
            {
                if (_WhereClause != "")
                {
                    return _WhereClause;
                }
                else
                {
                    String _str = String.Empty;

                    if (Conditions.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        string column = null;
                        string value = null;
                        int count = Conditions.Count;
                        int index = 0;
                        foreach (DictionaryEntry entry in Conditions)
                        {
                            column = Convert(entry.Key);
                            value = Convert(entry.Value);
                            _str += column + " = " + value;
                            index++;
                            if (index < count)
                                _str += " AND ";
                        }
                    }
                    return _str;
                }
            }
            set
            {
                _WhereClause = value;
            }
        }

        public PagedRequest()
        {
        }

        public PagedRequest(int pageIndex,
        int rowPerPage)
        {
            _pageIndex = pageIndex;
            _rowPerPage = rowPerPage;
        }
    }
}
