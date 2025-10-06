namespace GO.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using Models;

    public class FunctionService : BaseHttpClient, IFunctionService
    {
        public IList<Function> GetBySiteId(short siteId, bool? isActive = true)
        {
            var functions = Get<IList<Function>>(string.Format(RelativePaths.GetFunctionBySiteIdPath, siteId, isActive));
            return functions.OrderBy(x => x.DisplayOrder).ToList();
        }

        public async Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = true)
        {
            var functions = await GetAsync<IList<Function>>(string.Format(RelativePaths.GetFunctionBySiteIdPath, siteId, isActive));
            return functions.OrderBy(x => x.DisplayOrder).ToList();
        }

        public IList<Function> GetHierarchyBySiteId(short siteId, bool? isActive = true)
        {
            var functions = GetBySiteId(siteId, isActive);
            return GetHierarchyFunction(functions);
        }

        public async Task<IList<Function>> GetHierarchyBySiteIdAsync(short siteId, bool? isActive = true)
        {
            var functions = await GetBySiteIdAsync(siteId, isActive);
            return GetHierarchyFunction(functions);
        }

        public IList<FunctionPermission> GetPermissionBySiteAndRoleId(short siteId, short roleId)
            => Get<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionByRoleIdPath, siteId, roleId));

        public async Task<IList<FunctionPermission>> GetPermissionBySiteAndRoleIdAsync(short siteId, short roleId)
            => await GetAsync<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionByRoleIdPath, siteId, roleId));

        private static IList<Function> GetHierarchyFunction(IList<Function> functions)
        {
            var functionTrees = functions.Where(func => !func.ParentId.HasValue)
                .OrderBy(x => x.DisplayOrder).ToList();
            foreach (var function in functionTrees)
            {
                function.Level = 0;
                GetChildFunctions(function, functions);
            }

            return functionTrees;
        }

        private static void GetChildFunctions(Function function, IList<Function> functions)
        {
            var functionItems = functions.Where(x => x.ParentId == function.FunctionId)
                .OrderBy(x => x.DisplayOrder).ToList();
            if (functionItems.Any())
            {
                function.Childs = function.Childs ?? new List<Function>();

                foreach (var item in functionItems)
                {
                    item.Level = (byte)(function.Level + 1);
                    function.Childs.Add(item);
                    GetChildFunctions(item, functions);
                }
            }
        }

        private static class RelativePaths
        {
            public const string GetFunctionBySiteIdPath = "api/function/getBySiteId?siteId={0}&isActive={1}";
            public const string GetFunctionPermissionByRoleIdPath = "api/function/getPermissionBySiteAndRoleId?siteId={0}&roleId={1}";
        }
    }
}