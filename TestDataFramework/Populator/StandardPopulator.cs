using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Windsor.Diagnostics.Inspectors;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Persistence;
using TestDataFramework.TypeGenerator;
using TestDataFramework.ValueGenerator;

namespace TestDataFramework.Populator
{
    public class SetExpression<T>
    {
        public Expression<Func<T, object>> FieldExpression { get; set; }
        public object Value { get; set; }
    }

    public class StandardPopulator : IPopulator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (StandardPopulator));

        private readonly ITypeGenerator typeGenerator;
        private readonly IPersistence persistence;

        private readonly List<RecordReference> recordReferences = new List<RecordReference>();

        public StandardPopulator(ITypeGenerator typeGenerator, IPersistence persistence)
        {
            StandardPopulator.Logger.Debug("Entering constructor");

            this.typeGenerator = typeGenerator;
            this.persistence = persistence;

            StandardPopulator.Logger.Debug("Entering constructor");
        }

        #region Public Methods

        public virtual RecordReference<T> Add<T>()
            where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>()");

            RecordReference<T> result = this.Add((RecordReference)null, (ConcurrentDictionary<PropertyInfo, Action<T>>)null);

            StandardPopulator.Logger.Debug("Exiting Add<T>()");
            return result;
        }

        public virtual RecordReference<T> Add<T>(params Expression<Action<T>>[] assignmentLambdaExpressions) where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>(params Expression<Action<T>>[])");

            RecordReference<T> result = this.Add(null, assignmentLambdaExpressions);

            StandardPopulator.Logger.Debug("Exiting Add<T>(params Expression<Action<T>>[])");
            return result;
        }

        public virtual RecordReference<T> Add<T>(RecordReference primaryRecordReference,
            params Expression<Action<T>>[] assignmentLambdaExpressions) where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>(RecordReference, params Expression<Action<T>>[])");

            var propertyExpressionDictionary = new ConcurrentDictionary<PropertyInfo, Action<T>>();

            foreach (Expression<Action<T>> assignmentLambdaExpression in assignmentLambdaExpressions)
            {
                StandardPopulator.UpdateSetterDictionary(propertyExpressionDictionary, assignmentLambdaExpression);
            }

            RecordReference<T> result = this.Add(primaryRecordReference, propertyExpressionDictionary);

            StandardPopulator.Logger.Debug("Exiting Add<T>(RecordReference, params Expression<Action<T>>[])");
            return result;;
        }

        public virtual RecordReference<T> Add<T>(params SetExpression<T>[] setExpressions) where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>(params Expression<Action<T>>[])");

            RecordReference<T> result = this.Add(null, setExpressions);

            StandardPopulator.Logger.Debug("Exiting Add<T>(params Expression<Action<T>>[])");
            return result;
        }

        public virtual RecordReference<T> Add<T>(RecordReference primaryRecordReference,
            params SetExpression<T>[] setExpressions) where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>(RecordReference, params SetExpression<T>[] setExpressions)");

            var propertyExpressionDictionary = new ConcurrentDictionary<PropertyInfo, Action<T>>();

            foreach (SetExpression<T> setExpression in setExpressions)
            {
                StandardPopulator.UpdateSetterDictionary(propertyExpressionDictionary, setExpression);
            }

            RecordReference<T> result = this.Add(primaryRecordReference, propertyExpressionDictionary);

            StandardPopulator.Logger.Debug("Exiting Add<T>(RecordReference, params SetExpression<T>[] setExpressions)");
            return result;
        }

        public virtual IList<RecordReference<T>> Add<T>(int copies, params Expression<Action<T>>[] assignmentLambdaExpressions) where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>(int, params SetExpression<T>[] setExpressions)");

            IList<RecordReference<T>> result = this.Add(copies, null, assignmentLambdaExpressions);

            StandardPopulator.Logger.Debug("Exiting Add<T>(int, params SetExpression<T>[] setExpressions)");
            return result;
        }

        public virtual IList<RecordReference<T>> Add<T>(int copies, RecordReference primaryRecordReference, params Expression<Action<T>>[] assignmentLambdaExpressions) where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>(int, RecordReference, params Expression<Action<T>>[])");

            StandardPopulator.Logger.DebugFormat("Adding {0} types of {1} ", copies, typeof(T));

            var result = new List<RecordReference<T>>(copies);

            for (int i = 0; i < copies; i++)
            {
                result.Add(this.Add(primaryRecordReference, assignmentLambdaExpressions));
            }

            StandardPopulator.Logger.Debug("Exiting Add<T>(int, RecordReference, params Expression<Action<T>>[])");

            return result;
        }

        public virtual void Bind()
        {
            StandardPopulator.Logger.Debug("Entering Populate");

            this.persistence.Persist(this.recordReferences);
            this.recordReferences.Clear();

            StandardPopulator.Logger.Debug("Exiting Populate");
        }

        #endregion Public Methods

        #region Helpers

        private static void UpdateSetterDictionary<T>(
            ConcurrentDictionary<PropertyInfo, Action<T>> propertyExpressionDictionary,
            Expression<Action<T>> assignmentLambdaExpression)
        {
            if (assignmentLambdaExpression.Body.NodeType != ExpressionType.Assign)
            {
                throw new SetExpressionException(Messages.SetExpressionNotAssignment);
            }

            var assignmentExpression = (BinaryExpression)assignmentLambdaExpression.Body;

            var memberExpression = assignmentExpression.Left as MemberExpression;

            if (memberExpression == null)
            {
                throw new SetExpressionException(Messages.LValueMustBeProperty);
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new SetExpressionException(Messages.LValueMustBeProperty);
            }

            Action<T> setter = assignmentLambdaExpression.Compile();

            propertyExpressionDictionary.AddOrUpdate(propertyInfo, setter, (pi, lambda) => setter);
        }

        private static void UpdateSetterDictionary<T>(
            ConcurrentDictionary<PropertyInfo, Action<T>> propertyExpressionDictionary,
            SetExpression<T> setExpression)
        {
            var memberExpression = setExpression.FieldExpression.Body as MemberExpression;

            if (memberExpression == null)
            {
                var unaryExpression = setExpression.FieldExpression.Body as UnaryExpression;

                if (unaryExpression == null)
                {
                    throw new SetExpressionException(Messages.MustBePropertyAccess);
                }

                memberExpression = unaryExpression.Operand as MemberExpression;

                if (memberExpression == null)
                {
                    throw new SetExpressionException(Messages.MustBePropertyAccess);
                }
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            if (propertyInfo.GetSetMethod() == null)
            {
                throw new SetExpressionException(Messages.NoSetter);
            }

            Action<T> setter = @object => propertyInfo.SetValue(@object, setExpression.Value);

            propertyExpressionDictionary.AddOrUpdate(propertyInfo, setter, (pi, lambda) => setter);
        }

        #endregion Helpers

        private RecordReference<T> Add<T>(RecordReference primaryRecordReference,
            ConcurrentDictionary<PropertyInfo, Action<T>> propertyExpressionDictionary) where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>(primaryRecordReference, propertyExpressionDictionary)");

            StandardPopulator.Logger.Debug("Adding type " + typeof (T));

            this.typeGenerator.ResetRecursionGuard();

            object recordObject = propertyExpressionDictionary == null
                ? this.typeGenerator.GetObject(typeof (T))
                : this.typeGenerator.GetObject(propertyExpressionDictionary);

            var recordReference = new RecordReference<T>((T) recordObject, propertyExpressionDictionary?.Keys);

            this.recordReferences.Add(recordReference);

            if (primaryRecordReference != null)
            {
                recordReference.AddPrimaryRecordReference(primaryRecordReference);
            }

            StandardPopulator.Logger.Debug("Exiting Add<T>(primaryRecordReference, propertyExpressionDictionary)");

            return recordReference;
        }
    }
}