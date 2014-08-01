using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace CriterionMore
{
   
    internal  static class CriterionCache
    {
        static readonly ConcurrentDictionary<Type, MapCriterion> Dictionary = new ConcurrentDictionary<Type, MapCriterion>();
        internal static MapCriterion GetBaseMap(Type t)
        {
            MapCriterion f;
            return Dictionary.TryGetValue(t, out f) ? f : null;
        }

        internal static void Add(Type type, MapCriterion criterion)
        {
            Dictionary.GetOrAdd(type,criterion);
        }
    }
}