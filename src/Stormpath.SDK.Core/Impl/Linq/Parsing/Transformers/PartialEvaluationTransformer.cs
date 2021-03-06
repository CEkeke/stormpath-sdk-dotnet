﻿// <copyright file="PartialEvaluationTransformer.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stormpath.SDK.Impl.Linq.Parsing.Transformers
{
    /// <summary>
    /// Performs evaluation and replacement of independent sub-trees.
    /// Returns a new tree with sub-trees evaluated and replaced.
    /// </summary>
    internal sealed class PartialEvaluationTransformer : IExpressionTransformer
    {
        public Expression Transform(Expression expression)
        {
            var evaluator = new SubtreeEvaluator(
                new Nominator(CanBeEvaluatedLocally).Nominate(expression));

            return evaluator.Eval(expression);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        /// <summary>
        /// Evaluates and replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        private class SubtreeEvaluator : ExpressionVisitor
        {
            private readonly HashSet<Expression> candidates;
            private bool passedRootNode = false;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }

            public override Expression Visit(Expression expression)
            {
                if (expression == null)
                {
                    return null;
                }

                Expression result = null;

                if (this.candidates.Contains(expression))
                {
                    result = this.Evaluate(expression);
                }
                else
                {
                    result = base.Visit(expression);
                }

                // Skip the root node, only evaluate child nodes (if any)
                this.passedRootNode = true;

                return result;
            }

            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant ||
                    (!this.passedRootNode && e.NodeType == ExpressionType.Call))
                {
                    return e;
                }

                var compiled = Expression.Lambda(e).Compile();
                return Expression.Constant(compiled.DynamicInvoke(null), e.Type);
            }
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private readonly Func<Expression, bool> canBeEvaluatedFunc;
            private HashSet<Expression> candidates;
            private bool cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.canBeEvaluatedFunc = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                this.candidates = new HashSet<Expression>();
                this.Visit(expression);

                return this.candidates;
            }

            public override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                    this.cannotBeEvaluated = false;
                    base.Visit(expression);

                    if (!this.cannotBeEvaluated)
                    {
                        if (this.canBeEvaluatedFunc(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            this.cannotBeEvaluated = true;
                        }
                    }

                    this.cannotBeEvaluated |= saveCannotBeEvaluated;
                }

                return expression;
            }
        }
    }
}
