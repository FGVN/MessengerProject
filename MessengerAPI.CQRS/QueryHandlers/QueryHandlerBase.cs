using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Repositories;

namespace MessengerInfrastructure.QueryHandlers;

/// <summary>
/// Base class for query handlers providing common functionalities for handling queries.
/// </summary>
/// <typeparam name="TEntity">The type of entity being queried.</typeparam>
/// <typeparam name="TDTO">The type of DTO (Data Transfer Object) representing the query result.</typeparam>
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


    public virtual IEnumerable<string> GetFilterProperties(TEntity entity) 
    {
        Type entityType = entity.GetType();

        PropertyInfo[] properties = entityType.GetProperties();

        return properties.Select(p => p.Name);
    }

    /// <summary>
    /// Performs a search operation based on the provided query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>A collection of objects representing the search results.</returns>
    public virtual async Task<IEnumerable<object?>> SearchAsync(SearchQuery<TDTO> query)
    {

        SearchQueryValidator<TDTO> validator = new SearchQueryValidator<TDTO>();
        validator.ValidateAndThrow(query);

        if (query.Query == null || query.SortDirection == null)
            return new List<object>();

        IRepository<TEntity> repository = _unitOfWork.GetRepository<TEntity>();

        IEnumerable<string> filterProperties = GetFilterProperties();

        Expression<Func<TEntity, bool>> predicate = FilterEntities(filterProperties, query.Query);


        IQueryable<TEntity> queryable = repository.GetAllQueryable(predicate);

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
           .Select(item =>
           {
               var dynamicObject = Activator.CreateInstance(dynamicType);
               foreach (var prop in dynamicType.GetProperties().Where(prop => dtoProperties.Contains(prop.Name.ToLower())))
               {
                   var itemProp = Array.Find(typeof(TEntity).GetProperties(), p => string.Equals(p.Name, prop.Name, StringComparison.OrdinalIgnoreCase));
                   if (itemProp != null)
                   {
                       var value = itemProp.GetValue(item);
                       prop.SetValue(dynamicObject, value);
                   }
               }

               return dynamicObject;
           });

        return dynamicObjects.ToList();
    }

    /// <summary>
    /// Filters entities based on the provided properties and query string.
    /// </summary>
    /// <param name="properties">The properties to filter on.</param>
    /// <param name="query">The query string.</param>
    /// <returns>An expression representing the filter.</returns>
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

                // Check if the property is of type string or IEnumerable<string>
                if (propertyAccess.Type == typeof(string))
                {
                    // Handle string properties
                    var propertyToLower = Expression.Call(propertyAccess, "ToLower", null);
                    return Expression.Lambda<Func<TEntity, bool>>(Expression.Call(propertyToLower, "Contains", null, queryToLower), parameter);
                }
                else if (propertyAccess.Type.GetInterfaces().Contains(typeof(IEnumerable<string>)))
                {
                    // Handle IEnumerable<string> properties
                    var containsMethod = typeof(Enumerable).GetMethod("Contains", new[] { typeof(IEnumerable<string>), typeof(string) });

                    return Expression.Lambda<Func<TEntity, bool>>(
                        Expression.Call(containsMethod, propertyAccess, queryToLower),
                        parameter);
                }
                else
                {
                    // Convert non-string properties to string and perform the Contains operation
                    var toStringMethod = propertyAccess.Type.GetMethod("ToString", Type.EmptyTypes);
                    var propertyToLower = Expression.Call(Expression.Call(propertyAccess, toStringMethod), "ToLower", null);
                    return Expression.Lambda<Func<TEntity, bool>>(Expression.Call(propertyToLower, "Contains", null, queryToLower), parameter);
                }
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

    /// <summary>
    /// Creates a dynamic type based on the properties to retrieve and the DTO type.
    /// </summary>
    /// <param name="propertiesToRetrieve">The properties to retrieve.</param>
    /// <param name="dtoType">The type of the DTO.</param>
    /// <returns>The dynamically created type.</returns>
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
