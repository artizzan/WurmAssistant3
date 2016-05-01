using System;
using System.Linq.Expressions;

namespace AldursLab.WurmApi.Extensions.DotNet
{
    static class ReflectionHelper
    {
        public static Func<TClassType, TFieldType> GetFieldAccessor<TClassType, TFieldType>(string fieldName)
        {
            ParameterExpression param =
            Expression.Parameter(typeof(TClassType), "arg");

            MemberExpression member =
            Expression.Field(param, fieldName);

            LambdaExpression lambda =
            Expression.Lambda(typeof(Func<TClassType, TFieldType>), member, param);

            Func<TClassType, TFieldType> compiled = (Func<TClassType, TFieldType>)lambda.Compile();
            return compiled;
        }
    }
}
