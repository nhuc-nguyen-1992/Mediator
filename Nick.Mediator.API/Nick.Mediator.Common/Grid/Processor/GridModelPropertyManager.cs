using System.Reflection;
using Nick.Mediator.Common.Extensions;
using Nick.Mediator.Common.Grid.Info;

namespace Nick.Mediator.Common.Grid.Processor;

  public class GridModelPropertyManager
    {
        private static readonly Dictionary<Type, List<GridModelPropertyInfo>> GridPropertyDictionary = new Dictionary<Type, List<GridModelPropertyInfo>>();
        private static readonly Dictionary<string, Type> GridModelDictionary = new Dictionary<string, Type>();



        public List<GridModelPropertyInfo> GetAllColumnInfoByGridName(string gridName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var grids = executingAssembly.GetExportedTypes().Where(t => !t.IsInterface && typeof(IGridResponseItemModel).IsAssignableFrom(t)).ToList();
            var gridType = grids.FirstOrDefault(type => type.GetCustomAttribute<GridModelAttribute>()?.GridName == gridName);

            if (gridType != null)
                return GetAllColumnInfo(gridType);
            return null;
        }

        public List<string> GetAllGridNames()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var grids = executingAssembly.GetExportedTypes().Where(t => !t.IsInterface && typeof(IGridResponseItemModel).IsAssignableFrom(t)).ToList();
            var gridNames = grids.Select(type => type.GetCustomAttribute<GridModelAttribute>()?.GridName).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            return gridNames;
        }

        public List<GridModelPropertyInfo> GetAllColumnInfo<TGridType>()
            where TGridType : IGridResponseItemModel
        {
            return GetAllColumnInfo(typeof(TGridType));
        }



        public GridModelPropertyInfo GetColumnInfo<TGridType>(string propertyName)
            where TGridType : IGridResponseItemModel
        {
            return GetAllColumnInfo(typeof(TGridType)).FirstOrDefault(info => string.Equals(info.FieldName, propertyName, StringComparison.InvariantCultureIgnoreCase));
        }

        private List<GridModelPropertyInfo> GetAllColumnInfo(Type gridType)
        {
            lock ("_gridModeDictionary")
            {
                if (!GridPropertyDictionary.ContainsKey(gridType))
                {
                    var infos = ScanGridModelPropertyInfo(gridType);
                    GridPropertyDictionary.Add(gridType, infos);
                }

                return GridPropertyDictionary[gridType];
            }
        }

        private List<GridModelPropertyInfo> ScanGridModelPropertyInfo(Type type)
        {
            var properties = type.GetProperties();
            var infos = new List<GridModelPropertyInfo>();
            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<GridModelPropertyAttribute>();
                if (attr?.IsIgnored != true)
                {
                    var info = new GridModelPropertyInfo
                    {
                        FieldName = property.Name,
                        FieldNameCamelCase = property.Name.ToCamelCase(),
                        AllowFilter = !attr?.CannotFilter ?? true,
                        AllowSort = !attr?.CannotSort ?? true,
                        IgnoreFromExport = attr?.IgnoreFromExport ?? false,
                        DisplayName = attr?.DisplayName ?? property.Name,
                        InternalFieldName = attr?.InternalFieldName ?? property.Name,
                        DataType = property.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(property.PropertyType)?.Name.ToLower() : property.PropertyType?.Name.ToLower()
                    };
                    infos.Add(info);
                }
            }

            return infos;
        }
    }