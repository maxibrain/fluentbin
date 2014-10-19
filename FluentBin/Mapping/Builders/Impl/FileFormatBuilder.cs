using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using FluentBin.Mapping.Models;
using FluentBin.Mapping.Models.Impl;

namespace FluentBin.Mapping.Builders.Impl
{
    class FileFormatBuilder : IFileFormatBuilder
    {
        private readonly Endianness _endianness;

        public FileFormatBuilder()
        {
            Types = new Dictionary<Type, IExpressionBuilder>();
        }

        public FileFormatBuilder(Endianness endianness)
            : this()
        {
            _endianness = endianness;
        }

        public Dictionary<Type, IExpressionBuilder> Types { get; private set; }

        public IFileFormatBuilder Includes<T>(Action<ITypeBuilder<T>> typeBuilderConfiguration = null)
        {
            var typeBuilder = new TypeBuilder<T>();
            if (typeBuilderConfiguration != null)
                typeBuilderConfiguration(typeBuilder);
            Types[typeof (T)] = typeBuilder;
            return this;
        }

        public IFileFormat<T> Build<T>()
        {
            var lambdaExpression = GetLambdaExpression<T>(); 
            return new FileFormat<T>(lambdaExpression.Compile());
        }

        public void BuildToMethod<T>(MethodBuilder methodBuilder)
        {
            var lambdaExpression = GetLambdaExpression<T>();
            lambdaExpression.CompileToMethod(methodBuilder);
        }

        private Expression<ReadDelegate<T>> GetLambdaExpression<T>()
        {
            var brParameter = Expression.Parameter(typeof(BitsReader), "br");

            Dictionary<Type, Expression<ReadFunc>> typeReaders = Types.ToDictionary(pair => pair.Key, pair =>
                {
                    var args = new ExpressionBuilderArgs();
                    args.BrParameter = brParameter;
                    args.InstanceVar = Expression.Parameter(typeof (Object));
                    args.TypeReaders = Expression.Parameter(typeof (Dictionary<Type, Expression<ReadFunc>>));
                    args.Endianness = Expression.Constant(_endianness);
                    var expression = pair.Value.BuildExpression(args);
                    return Expression.Lambda<ReadFunc>(expression, string.Format("Read_{0}", pair.Key.Name),
                                                       new[] {args.BrParameter, args.InstanceVar, args.TypeReaders});
                });

            var resultVar = Expression.Variable(typeof(T), "root");
            var brCheckExpression = Expression.IfThen(Expression.Equal(brParameter, Expression.Constant(null)),
                                        Expression.Throw(Expression.New(typeof(ArgumentNullException).GetConstructor(new[] { typeof(String) }), Expression.Constant(brParameter.Name, typeof(String)))));
            var ctorExpression = Expression.Assign(resultVar, Expression.New(typeof(T)));
            var typeReadersVar = Expression.Variable(typeof(Dictionary<Type, Expression<ReadFunc>>));
            var typeReadExpression = Expression.Block(new[] { resultVar, typeReadersVar },
                brCheckExpression,
                CreateTypeReaders(typeReadersVar, typeReaders),
                ctorExpression,
                Expression.Invoke(AdvancedExpression.GetTypeBuilder(typeReadersVar, Expression.Constant(typeof(T))),
                                  brParameter, resultVar, typeReadersVar),
                resultVar);

            return Expression.Lambda<ReadDelegate<T>>(typeReadExpression, string.Format("Read_Root", typeof(T).Name), new[] { brParameter });
        }

        private Expression CreateTypeReaders(ParameterExpression typeReadersVar, Dictionary<Type, Expression<ReadFunc>> typeReaders)
        {
            var expressions = new List<Expression>();
            expressions.Add(Expression.Assign(typeReadersVar, Expression.New(typeof(Dictionary<Type, Expression<ReadFunc>>))));
            expressions.AddRange(typeReaders.Select(typeReader => Expression.Call(typeReadersVar, "Add", null, Expression.Constant(typeReader.Key), Expression.Constant(typeReader.Value))));
            return Expression.Block(expressions);
        }
    }
}
