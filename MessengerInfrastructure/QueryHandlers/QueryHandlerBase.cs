using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Repositories;

namespace MessengerInfrastructure.QueryHandlers;

public abstract class QueryHandlerBase<TEntity, TDTO>
    where TEntity : class
    where TDTO : class
{
    protected readonly IUnitOfWork _unitOfWork;

    protected QueryHandlerBase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public virtual IQueryable<TEntity> GetAllQueryable(Expression<Func<TEntity, bool>> predicate)
    {
        var dbContext = _unitOfWork.Context;
        var queryable = dbContext.Set<TEntity>().AsQueryable();
        return predicate != null ? queryable.Where(predicate) : queryable;
    }

    protected abstract IEnumerable<string> GetFilterProperties(TEntity entity);

    public virtual async Task<IEnumerable<object?>> SearchAsync(SearchQuery<TDTO> query)
    {

        SearchQueryValidator<TDTO> validator = new SearchQueryValidator<TDTO>();
        validator.ValidateAndThrow(query);

        if (query.Query == null || query.SortDirection == null)
            return new List<object>();

        IRepository<TEntity> userRepository = _unitOfWork.GetRepository<TEntity>();

        IEnumerable<string> filterProperties = GetFilterProperties();

        Expression<Func<TEntity, bool>> userPredicate = FilterEntities(filterProperties, query.Query);


        IQueryable<TEntity> queryable = userRepository.GetAllQueryable(userPredicate);

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            queryable = SortEntities(queryable, query.SortBy, query.SortDirection);
        }

        IEnumerable<string> propertiesToRetrieve = query.PropertiesToRetrieve != null && query.PropertiesToRetrieve.Any() ?
            query.PropertiesToRetrieve.Select(p => p.ToLower()) :
            typeof(TDTO).GetProperties().Select(p => p.Name.ToLower());

        Type dynamicType = CreateDynamicType(propertiesToRetrieve, typeof(TDTO));

        List<string> dtoProperties = typeof(TDTO).GetProperties().Select(p => p.Name.ToLower()).ToList();

        IEnumerable<object?> dynamicObjects = (await queryable
           .Skip(query.From)
           .Take(query.To - query.From)
           .ToListAsync())
           .Select(user =>
           {
               var dynamicObject = Activator.CreateInstance(dynamicType);
               foreach (var prop in dynamicType.GetProperties().Where(prop => dtoProperties.Contains(prop.Name.ToLower())))
               {
                   var userProp = Array.Find(typeof(TEntity).GetProperties(), p => string.Equals(p.Name, prop.Name, StringComparison.OrdinalIgnoreCase));
                   if (userProp != null)
                   {
                       var value = userProp.GetValue(user);
                       prop.SetValue(dynamicObject, value);
                   }
               }

               return dynamicObject;
           });

        return dynamicObjects.ToList();
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

            var propertyExpressions = properties.Select<string, Expression<Func<TEntity, bool>>>(prop =>
            {
                var propertyAccess = Expression.Property(parameter, prop);
                Expression propertyToLower;
                if (propertyAccess.Type == typeof(string))
                {
                    propertyToLower = Expression.Call(propertyAccess, "ToLower", null);
                }
                else
                {
                    var toStringMethod = propertyAccess.Type.GetMethod("ToString", Type.EmptyTypes);
                    propertyToLower = Expression.Call(propertyAccess, toStringMethod);
                }

                return Expression.Lambda<Func<TEntity, bool>>(Expression.Call(propertyToLower, "Contains", null, queryToLower), parameter);
            });

            var body = propertyExpressions.Aggregate<Expression<Func<TEntity, bool>>, Expression>(null, (current, propertyExpression) =>
            {
                if (current == null)
                {
                    return propertyExpression.Body;
                }
                else
                {
                    var orElse = Expression.OrElse(current, propertyExpression.Body);
                    return Expression.Lambda<Func<TEntity, bool>>(orElse, parameter).Body;
                }
            });

            return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
        }
    }

    protected IEnumerable<string> GetFilterProperties()
    {
        var properties = typeof(TDTO).GetProperties();
        return properties.Select(prop => prop.Name);
    }

    protected IQueryable<TEntity> SortEntities(IQueryable<TEntity> queryable, string sortBy, string sortDirection)
    {
        var propertyInfo = Array.Find(typeof(TEntity).GetProperties(),
             p => string.Equals(p.Name, sortBy, StringComparison.OrdinalIgnoreCase));

        if (propertyInfo != null)
        {
            return sortDirection.ToLower() == "asc" ?
                queryable.OrderBy(u => EF.Property<object>(u, propertyInfo.Name)) :
                queryable.OrderByDescending(u => EF.Property<object>(u, propertyInfo.Name));
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

        if (assemblyName.Name == null)
            throw new ArgumentNullException("Assebmly name cannot be null", new Exception());

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
        if (typeBuilder.CreateType() == null)
            throw new ArgumentNullException("Created type is null", new Exception());

        return typeBuilder.CreateType();
    }
}
