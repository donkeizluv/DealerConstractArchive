﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DealContractArchiver.ViewModels.Helper
{
    public static class ExpressionHelper
    {
      
        public static Expression<Func<T, bool>> GetContainsExpression<T>(string propertyName, string containsValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "type");
            var propertyExp = Expression.Property(parameterExp, propertyName);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            if (propertyExp.Type == typeof(Int32)) //incase column is int
            {
                method = typeof(Int32).GetMethod("Equals", new[] { typeof(Int32) });
                int.TryParse(containsValue, out int value);
                var someIntValue = Expression.Constant(value, typeof(Int32));
                var containsIntMethodExp = Expression.Call(propertyExp, method, someIntValue);
                return Expression.Lambda<Func<T, bool>>(containsIntMethodExp, parameterExp);
            }

            var someValue = Expression.Constant(containsValue, typeof(string));
            var containsMethodExp = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);
        }

        public static Expression<Func<T, TKey>> GetPropertyExpression<T, TKey>(string propertyName)
        {
            var parameterExp = Expression.Parameter(typeof(T), "type");
            var exp = Expression.Property(parameterExp, propertyName);
            return Expression.Lambda<Func<T, TKey>>(exp, parameterExp);
        }
    }
}