using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using FluentValidation;

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
        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var dbContext = _unitOfWork.Context;
            return dbContext.Set<TEntity>().Where(predicate).ToList();
        }
        protected abstract IEnumerable<string> GetFilterProperties(TEntity entity);
        public virtual async Task<IEnumerable<object>> SearchAsync(SearchQuery<TDTO> query)
        {
            var validator = new SearchQueryValidator<TDTO>();
            validator.ValidateAndThrow(query);

            var userRepository = _unitOfWork.GetQueryRepository<TEntity>();

            var filterProperties = GetFilterProperties();

            var userPredicate = FilterEntities(filterProperties, query.Query);

            var sortedUsers = await userRepository.GetAllAsync(userPredicate);
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                sortedUsers = SortEntities(sortedUsers, query.SortBy, query.SortDirection);
            }

            var propertiesToRetrieve = query.PropertiesToRetrieve != null && query.PropertiesToRetrieve.Any() ?
                query.PropertiesToRetrieve.Select(p => p.ToLower()) :
                typeof(TDTO).GetProperties().Select(p => p.Name.ToLower());

            var dynamicType = CreateDynamicType(propertiesToRetrieve, typeof(TDTO));

            var dtoProperties = typeof(TDTO).GetProperties().Select(p => p.Name.ToLower()).ToList();

            var dynamicObjects = sortedUsers
                .Skip(query.From)
                .Take(query.To - query.From)
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


        protected Expression<Func<TEntity, bool>> FilterEntities(IEnumerable<string> properties, string query)
        {
            if (string.IsNullOrEmpty(query) || !properties.Any())
            {
                return _ => true; 
            }
            else
            {
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var queryToLower = Expression.Constant(query.ToLower());

                var propertyExpressions = properties.Select(prop =>
                {
                    var propertyAccess = Expression.Property(parameter, prop);
                    var propertyToLower = Expression.Call(propertyAccess, "ToLower", null);
                    return Expression.Call(propertyToLower, "Contains", null, queryToLower);
                });

                var body = propertyExpressions.Aggregate<Expression, Expression>(null, (current, propertyExpression) =>
                    current == null ? propertyExpression : Expression.OrElse(current, propertyExpression));

                return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
            }
        }
        protected IEnumerable<string> GetFilterProperties()
        {
            var properties = typeof(TDTO).GetProperties();
            return properties.Select(prop => prop.Name);
        }


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
