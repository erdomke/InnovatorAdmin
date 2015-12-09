using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection;

namespace InnovatorAdmin.Scripts
{
  public class FastObjectFactory
  {
    private static readonly object _syncRoot = new object();

    private static readonly Dictionary<string, Delegate> _creatorCache = new Dictionary<string, Delegate>();
    public static T CreateObject<T>() where T : class, new()
    {
      return CreateObject<T>(typeof(T));
    }

    public static T CreateObject<T>(Type concreteType)
    {
      Func<T> func = null;
      Delegate del = null;
      var abstractType = typeof(T);

      if (!concreteType.Equals(abstractType) && !abstractType.IsAssignableFrom(concreteType))
        throw new ArgumentException("Type mismatch");

      if (_creatorCache.TryGetValue(concreteType.AssemblyQualifiedName, out del))
      {
        func = (Func<T>)del;
      }
      else
      {
        lock (_syncRoot)
        {
          var dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + Guid.NewGuid().ToString("N"), abstractType, null, concreteType);
          var ilGen = dynMethod.GetILGenerator();
          ilGen.Emit(OpCodes.Newobj, concreteType.GetConstructor(System.Type.EmptyTypes));
          ilGen.Emit(OpCodes.Ret);
          func = (Func<T>)dynMethod.CreateDelegate(typeof(Func<T>));
          _creatorCache[concreteType.AssemblyQualifiedName] = func;
        }
      }

      return func.Invoke();
    }

    public static T CreateObject<T, S>(S arg) where T : class
    {
      Func<S, T> func = null;
      Delegate del = null;
      var type = typeof(T);
      var argTypes = new System.Type[] { typeof(S) };

      if (_creatorCache.TryGetValue(type.AssemblyQualifiedName + argTypes[0].AssemblyQualifiedName, out del))
      {
        func = (Func<S, T>)del;
      }
      else
      {
        lock (_syncRoot)
        {
          func = GetConstructor<T, S>(type, argTypes);
          _creatorCache[type.AssemblyQualifiedName + argTypes[0].AssemblyQualifiedName] = func;
        }
      }

      return func.Invoke(arg);
    }

    public static Func<S, T> GetConstructor<T, S>(System.Type type, System.Type[] argTypes) where T : class
    {
      var dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + Guid.NewGuid().ToString("N"), type, argTypes, type);
      var ilGen = dynMethod.GetILGenerator();
      ilGen.Emit(OpCodes.Ldarg_0);
      ilGen.Emit(OpCodes.Newobj, type.GetConstructor(argTypes));
      ilGen.Emit(OpCodes.Ret);
      return (Func<S, T>)dynMethod.CreateDelegate(typeof(Func<S, T>));
    }
  }
}
