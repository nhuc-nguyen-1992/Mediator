using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using Nick.Mediator.Common.Extensions;
using Nick.Mediator.Common.Grid.Info;
using Nick.Mediator.Common.Grid.Request;

namespace Nick.Mediator.Common.Grid.Processor;

public class CommonGridHandler
    {
        private const int DefaultMinPageIndex = 1;
        private const int DefaultMinPageSize = 1;
        private readonly GridModelPropertyManager _gridModelPropertyManager;
        public CommonGridHandler()
        {
            _gridModelPropertyManager = new GridModelPropertyManager();//rushing, no DI here
        }

        public async Task<List<string>> GetDistinctValues<TModelType, TResponseItemType>(IQueryable<TModelType> query, BaseGridDistinctRequest request, CancellationToken cancellationToken = default) where TResponseItemType : IGridResponseItemModel
        {
            IQueryable currentQuery = query;
            //filter
            currentQuery = await ApplyFilters<TResponseItemType>(currentQuery, request?.Filters);

            //selector
            var resultSet = ApplyDistinctSelector<TResponseItemType>(currentQuery, request?.FieldName);

            //execute
            if (resultSet == null) return new List<string>();
            var objs = await resultSet.Take(51).ToDynamicListAsync<object>();
            var values = objs.Select(o => o?.ToString()).ToList();
            return values;
        }

        public async Task<TResponseType> GetGridData<TModelType, TResponseType, TResponseItemType>(IQueryable<TModelType> query, Request.BaseGridRequest request, CancellationToken cancellationToken = default)
            where TResponseType : Response.BaseGridResponse<TResponseItemType>, new() where TResponseItemType : IGridResponseItemModel
        {
            IQueryable currentQuery = query;
            //filter
            currentQuery = await ApplyFilters<TResponseItemType>(currentQuery, request?.Filters);

            //now, we do some magic for paging
            var totalRecord = currentQuery.Count();

            //we can re-calculate the paging info here
            var pagingInfo = new Response.GridPagingInfo { CurrentPageIndex = request?.PageIndex ?? 1, CurrentPageSize = request?.PageSize ?? 10, TotalRecord = totalRecord };
            pagingInfo.CurrentPageIndex = Math.Max(pagingInfo.CurrentPageIndex, DefaultMinPageIndex);
            pagingInfo.CurrentPageSize = Math.Max(pagingInfo.CurrentPageSize, DefaultMinPageSize);

            //sort
            currentQuery = ApplySorts<TResponseItemType>(currentQuery, request?.Sort);

            //paging
            currentQuery = currentQuery.Page(pagingInfo.CurrentPageIndex, pagingInfo.CurrentPageSize);

            //selector
            var resultSet = ApplySelectors<TResponseItemType>(currentQuery, request?.Columns);

            //execute
            var response = new TResponseType
            {
                Paging = pagingInfo,
                GridData = await resultSet.ToDynamicListAsync<TResponseItemType>()
            };

            return response;
        }

        public async Task<TResponseType> GetGridDataTModel<TModelType, TResponseType, TResponseItemType>(IQueryable<TModelType> query, Request.BaseGridRequest request, CancellationToken cancellationToken = default)
            where TResponseType : Response.BaseGridResponse<TResponseItemType>, new() where TResponseItemType : IGridResponseItemModel
        {
            IQueryable currentQuery = query;
            //filter
            currentQuery = await ApplyFilters<TResponseItemType>(currentQuery, request?.Filters);

            //now, we do some magic for paging
            var totalRecord = await currentQuery.CountAsync(cancellationToken);

            //we can re-calculate the paging info here
            var pagingInfo = new Response.GridPagingInfo { CurrentPageIndex = request?.PageIndex ?? 1, CurrentPageSize = request?.PageSize ?? 10, TotalRecord = totalRecord };
            pagingInfo.CurrentPageIndex = Math.Max(pagingInfo.CurrentPageIndex, DefaultMinPageIndex);
            pagingInfo.CurrentPageSize = Math.Max(pagingInfo.CurrentPageSize, DefaultMinPageSize);

            //sort
            currentQuery = ApplySorts<TResponseItemType>(currentQuery, request?.Sort);

            //paging
            currentQuery = currentQuery.Page(pagingInfo.CurrentPageIndex, pagingInfo.CurrentPageSize);

            //selector
            var resultSet = ApplySelectors<TResponseItemType>(currentQuery, request?.Columns);

            //execute
            var response = new TResponseType
            {
                Paging = pagingInfo,
                GridData = await resultSet.ToDynamicListAsync<TResponseItemType>()
            };

            return response;
        }

        private IQueryable ApplySorts<TResponseItemType>(IQueryable query, List<GridSortRequest> sorts)
            where TResponseItemType : IGridResponseItemModel
        {
            if (sorts?.Any() != true)
                return query;

            var columnSortStatements = new List<string>();

            foreach (var sortItem in sorts)
            {
                var columnInfo = _gridModelPropertyManager.GetColumnInfo<TResponseItemType>(sortItem.FieldName);
                if (columnInfo != null)
                    columnSortStatements.Add(
                        $"{columnInfo.InternalFieldName} {(sortItem.IsAscending ? "asc" : "desc")}");
            }

            if (!columnSortStatements.Any()) return query;

            var sortStatement = string.Join(", ", columnSortStatements);
            var resultSet = query.OrderBy(sortStatement);

            return resultSet;
        }


        private IQueryable ApplyDistinctSelector<TResponseItemType>(IQueryable query, string requestFieldName) where TResponseItemType : IGridResponseItemModel
        {
            var columnInfo = _gridModelPropertyManager.GetColumnInfo<TResponseItemType>(requestFieldName);
            if (columnInfo == null)
                return null;

            var resultSet = query.Select<object>($"{columnInfo.InternalFieldName}").Distinct();
            return resultSet;
        }

        private IQueryable ApplySelectors<TResponseItemType>(IQueryable query, List<GridColumnRequest> columnRequests) where TResponseItemType : IGridResponseItemModel
        {
            var selectedColumns = new List<GridModelPropertyInfo>();
            columnRequests ??= new List<GridColumnRequest>();
            foreach (var columnRequest in columnRequests)
            {
                var columnInfo = _gridModelPropertyManager.GetColumnInfo<TResponseItemType>(columnRequest.FieldName);
                if (columnInfo != null)
                    selectedColumns.Add(columnInfo);
            }

            if (columnRequests.Any() != true)
            {
                //they don't select any fields, or all selected field are invalid, we will response all data
                selectedColumns.AddRange(_gridModelPropertyManager.GetAllColumnInfo<TResponseItemType>());
            }

            var columnSelectStatements = new List<string>();
            foreach (var info in selectedColumns)
            {
                columnSelectStatements.Add($"{info.InternalFieldName} as {info.FieldName}");
            }

            var selectorStatement = $"new {{ {string.Join(", ", columnSelectStatements)} }}";

            var resultSet = query.Select<TResponseItemType>(selectorStatement);
            return resultSet;
        }

        private async Task<IQueryable> ApplyFilters<TResponseItemType>(IQueryable query, List<GridFilterRequest> filters) where TResponseItemType : IGridResponseItemModel
        {
            if (filters?.Any() != true)
                return query;

            var whereStatements = new List<string>();
            var whereParams = new List<object>();
            foreach (var filter in filters)
            {
                var condition = GetCondition<TResponseItemType>(filter, whereParams);
                if (!string.IsNullOrWhiteSpace(condition))
                    whereStatements.Add(condition);
            }

            if (whereStatements.Any())
            {
                var whereStatement = string.Join(" && ", whereStatements);
                //var config = new ParsingConfig { ResolveTypesBySimpleName = true };
                query = query.Where(whereStatement, whereParams.ToArray());
            }

            return query;
        }

        private string GetCondition<TResponseItemType>(GridFilterRequest filter, List<object> paramList) where TResponseItemType : IGridResponseItemModel
        {
            var columnInfo = _gridModelPropertyManager.GetColumnInfo<TResponseItemType>(filter.FieldName);
            if (columnInfo == null)
                return null;

            var result = "";
            switch (filter.FilterType)
            {
                case GridFilterType.Equal:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)} = @{paramList.Count}";
                    paramList.Add(filter.Value);
                    break;
                case GridFilterType.LessThan:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)} < @{paramList.Count}";
                    paramList.Add(filter.Value);
                    break;
                case GridFilterType.LessThanOrEqual:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)} <= @{paramList.Count}";
                    paramList.Add(filter.Value);
                    break;
                case GridFilterType.GreaterThan:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)} > @{paramList.Count}";
                    paramList.Add(filter.Value);
                    break;
                case GridFilterType.GreaterThanOrEqual:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)} >= @{paramList.Count}";
                    paramList.Add(filter.Value);
                    break;
                case GridFilterType.Between:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)} >= @{paramList.Count} && {GetLocalFieldName(columnInfo.InternalFieldName)} <= @{paramList.Count + 1} ";
                    paramList.Add(filter.Value);
                    paramList.Add(filter.OtherValue);
                    break;
                case GridFilterType.NotEqual:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)} != @{paramList.Count}";
                    paramList.Add(filter.Value);
                    break;
                case GridFilterType.IsNull:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)} == null";
                    paramList.Add(filter.Value);
                    break;
                case GridFilterType.IsNotNull:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)} != null";
                    paramList.Add(filter.Value);
                    break;
                case GridFilterType.In:
                    if (string.Equals(columnInfo.DataType, nameof(Boolean), StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = $"@{paramList.Count}.Contains({GetLocalFieldName(columnInfo.InternalFieldName)})";
                        paramList.Add(filter.Values.Select(TConverter.ChangeType<bool>).ToList());
                        break;
                    }
                    else if (string.Equals(columnInfo.DataType, nameof(Int32), StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(columnInfo.DataType, nameof(Decimal), StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = $"@{paramList.Count}.Contains(({GetLocalFieldName(columnInfo.InternalFieldName)}).ToString())";
                        paramList.Add(filter.Values);
                        break;
                    }
                    else if (string.Equals(columnInfo.DataType, nameof(String), StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = $"@{paramList.Count}.Contains({GetLocalFieldName(columnInfo.InternalFieldName)})";
                        paramList.Add(filter.Values);
                        //paramList.Add(filter.Values.ConvertAll(d => d.ToLower()));
                        break;
                    }

                    result = $"@{paramList.Count}.Contains({GetLocalFieldName(columnInfo.InternalFieldName)})";
                    paramList.Add(filter.Values);
                    break;
                case GridFilterType.Like:
                    result = $"{GetLocalFieldName(columnInfo.InternalFieldName)}.Contains(@{paramList.Count})";
                    paramList.Add(filter.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        private string GetLocalFieldName(string fieldName)
        {
            return fieldName;
        }
    }