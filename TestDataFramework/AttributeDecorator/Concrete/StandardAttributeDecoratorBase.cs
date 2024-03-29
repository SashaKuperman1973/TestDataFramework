﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;

namespace TestDataFramework.AttributeDecorator.Concrete
{
    public class StandardAttributeDecoratorBase : IAttributeDecoratorBase
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardAttributeDecoratorBase));

        protected readonly ConcurrentDictionary<MemberInfoProxy, List<Attribute>> MemberAttributeDicitonary =
            new ConcurrentDictionary<MemberInfoProxy, List<Attribute>>();

        protected string DefaultSchema;

        public StandardAttributeDecoratorBase()
        {
            this.DefaultSchema = null;
        }

        public StandardAttributeDecoratorBase(Schema defaultSchema)
        {
            this.DefaultSchema = defaultSchema.Value;
        }

        public virtual T GetSingleAttribute<T>(MemberInfoProxy memberInfo) where T : Attribute
        {
            StandardAttributeDecoratorBase.Logger.Debug(
                $"Entering GetSingleAttribute. T: {typeof(T)} memberInfo: {memberInfo.GetExtendedMemberInfoString()}");

            T[] result = this.GetCustomAttributes<T>(memberInfo).ToArray();

            if (result.Length <= 1)
            {
                T firstOrDefaultResult = result.FirstOrDefault();

                StandardAttributeDecoratorBase.Logger.Debug(
                    $"Member attributes count <= 1. firstOrDefaultResult: {firstOrDefaultResult}");
                return firstOrDefaultResult;
            }

            string message =
                memberInfo.MemberType == MemberTypes.Property
                    ? Messages.AmbigousPropertyAttributeMatch
                    : memberInfo.MemberType == MemberTypes.TypeInfo || memberInfo.MemberType == MemberTypes.NestedType
                        ? Messages.AmbigousTypeAttributeMatch
                        : Messages.AmbigousAttributeMatch;

            throw new AmbiguousMatchException(string.Format(message, typeof(T), memberInfo.Name,
                memberInfo.DeclaringType));
        }

        public virtual IEnumerable<T> GetCustomAttributes<T>(MemberInfoProxy memberInfo) where T : Attribute
        {
            StandardAttributeDecoratorBase.Logger.Debug(
                $"Entering GetCustomAttributes<T>. T: {typeof(T)}. memberInfo: {memberInfo}");

            List<Attribute> programmaticAttributeList;

            List<Attribute> attributeResult =
                this.MemberAttributeDicitonary.TryGetValue(memberInfo, out programmaticAttributeList)
                    ? programmaticAttributeList.Where(a => a.GetType() == typeof(T)).ToList()
                    : new List<Attribute>();

            attributeResult.AddRange(memberInfo.GetCustomAttributes<T>());

            List<T> result = this.InsertDefaultSchema(attributeResult).Cast<T>().ToList();

            StandardAttributeDecoratorBase.Logger.Debug("Exiting GetCustomAttributes<T>");
            return result;
        }

        public virtual IEnumerable<Attribute> GetCustomAttributes(MemberInfoProxy memberInfo)
        {
            StandardAttributeDecoratorBase.Logger.Debug(
                $"Entering GetCustomAttributes. MemberInfoProxy: {memberInfo}");

            List<Attribute> result;

            if (!this.MemberAttributeDicitonary.TryGetValue(memberInfo, out result))
                result = new List<Attribute>();

            result.AddRange(memberInfo.GetCustomAttributes());

            result = this.InsertDefaultSchema(result);

            StandardAttributeDecoratorBase.Logger.Debug("Exiting GetCustomAttributes.");
            return result;
        }

        private List<Attribute> InsertDefaultSchema(IEnumerable<Attribute> attributes)
        {
            StandardAttributeDecoratorBase.Logger.Debug("Entering InsertDefaultSchema");

            attributes = attributes.ToList();

            StandardAttributeDecoratorBase.Logger.Debug(
                $"Attributes: {string.Join(",", attributes)}");

            List<Attribute> result = attributes.Select(a =>
            {
                var canHaveDefaultSchema = a as ICanHaveDefaultSchema;

                Attribute resultAttribute = canHaveDefaultSchema?.IsDefaultSchema ?? false
                    ? canHaveDefaultSchema.GetAttributeUsingDefaultSchema(this.DefaultSchema)
                    : a;

                return resultAttribute;
            }).ToList();

            StandardAttributeDecoratorBase.Logger.Debug("Exiting InsertDefaultSchema");
            return result;
        }
    }
}