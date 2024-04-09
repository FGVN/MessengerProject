using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using FluentValidation;
using DataAccess.Models.Users;
using DataDomain.Users;

namespace MessengerInfrastructure.Services
{
    public abstract class QueryHandlerBase<TEntity, TDTO>
        where TEntity : class
        where TDTO : class
    {
        protected readonly IUnitOfWork _unitOfWork;

        public QueryHandlerBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public abstract Task<IEnumerable<TDTO>> GetAllAsync();

        public virtual async Task<IEnumerable<object>> SearchAsync(SearchQuery<TDTO> query)
        {
            var validator = new SearchQueryValidator<TDTO>();
            validator.ValidateAndThrow(query);

            var userRepository = _unitOfWork.GetQueryRepository<TEntity>();
            var users = await userRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(query.Query))
            {
                users = FilterEntities(users, query.Query);
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                users = SortEntities(users, query.SortBy, query.SortDirection);
            }

            var propertiesToRetrieve = query.PropertiesToRetrieve != null && query.PropertiesToRetrieve.Any() ?
                query.PropertiesToRetrieve.Select(p => p.ToLower()) :
                typeof(TDTO).GetProperties().Select(p => p.Name.ToLower());

            var dynamicType = CreateDynamicType(propertiesToRetrieve, typeof(TDTO));

            var dtoProperties = typeof(TDTO).GetProperties().Select(p => p.Name.ToLower()).ToList();

            var dynamicObjects = PaginateEntities(users, query.From, query.To)
                .Select(user =>
                {
                    var dynamicObject = Activator.CreateInstance(dynamicType);
                    foreach (var prop in dynamicType.GetProperties())
                    {
                        if (dtoProperties.Contains(prop.Name.ToLower()))
                        {
                            var userProp = typeof(TEntity).GetProperties().FirstOrDefault(p => string.Equals(p.Name, prop.Name, StringComparison.OrdinalIgnoreCase));
                            if (userProp != null)
                            {
                                var value = userProp.GetValue(user);
                                prop.SetValue(dynamicObject, value);
                            }
                        }
                    }
                    return dynamicObject;
                });

            return dynamicObjects;
        }

        protected IEnumerable<TEntity> FilterEntities(IEnumerable<TEntity> entities, string query)
        {
            var lowercaseQuery = query.ToLower();
            return entities.Where(x =>
                GetFilterProperties(x).Any(prop =>
                    prop.ToLower().Contains(lowercaseQuery)
                )
            );
        }

        protected abstract IEnumerable<string> GetFilterProperties(TEntity entity);

        protected IEnumerable<TEntity> SortEntities(IEnumerable<TEntity> entities, string sortBy, string sortDirection)
        {
            var propertyInfo = typeof(TEntity).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                return sortDirection.ToLower() == "asc" ?
                    entities.OrderBy(u => propertyInfo.GetValue(u)) :
                    entities.OrderByDescending(u => propertyInfo.GetValue(u));
            }
            else
            {
                throw new ArgumentException($"Invalid SortBy option: {sortBy}");
            }
        }

        protected IEnumerable<TEntity> PaginateEntities(IEnumerable<TEntity> entities, int from, int to)
        {
            return entities.Skip(from).Take(Math.Min(to - from, entities.Count()));
        }

        protected Type CreateDynamicType(IEnumerable<string>? propertiesToRetrieve, Type dtoType)
        {
            var assemblyName = new AssemblyName("DynamicAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            var typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public);

            // Define properties based on the propertiesToRetrieve list
            var dtoProperties = dtoType.GetProperties().Select(p => p.Name.ToLower()).ToList();
            if (propertiesToRetrieve != null)
            {
                foreach (var propName in propertiesToRetrieve)
                {
                    if (!dtoProperties.Contains(propName.ToLower()))
                        continue;

                    var propertyBuilder = typeBuilder.DefineProperty(propName, PropertyAttributes.None, typeof(object), null);
                    var fieldBuilder = typeBuilder.DefineField($"_{propName}", typeof(object), FieldAttributes.Private);

                    var getMethodBuilder = typeBuilder.DefineMethod($"get_{propName}",
                                                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                                typeof(object), Type.EmptyTypes);
                    var getIL = getMethodBuilder.GetILGenerator();
                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                    getIL.Emit(OpCodes.Ret);

                    var setMethodBuilder = typeBuilder.DefineMethod($"set_{propName}",
                                                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                                null, new Type[] { typeof(object) });
                    var setIL = setMethodBuilder.GetILGenerator();
                    setIL.Emit(OpCodes.Ldarg_0);
                    setIL.Emit(OpCodes.Ldarg_1);
                    setIL.Emit(OpCodes.Stfld, fieldBuilder);
                    setIL.Emit(OpCodes.Ret);

                    propertyBuilder.SetGetMethod(getMethodBuilder);
                    propertyBuilder.SetSetMethod(setMethodBuilder);
                }
            }

            return typeBuilder.CreateType();
        }
    }
}
