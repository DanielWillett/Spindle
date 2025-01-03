﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Core.Utils;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public static class RocketHelper
{
    public static bool IsUri(string uri)
    {
        if (string.IsNullOrEmpty(uri) || !Uri.TryCreate(uri, UriKind.Absolute, out Uri result))
            return false;

        return result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps;
    }

    public static List<Type> GetTypes(List<Assembly> assemblies)
    {
        List<Type> allTypes = new List<Type>();
        foreach (Assembly assembly in assemblies)
        {
            Type[] asmTypes;
            try
            {
                asmTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                asmTypes = ex.Types;
            }

            allTypes.AddRange(asmTypes);
        }

        return allTypes;
    }

    public static List<Type> GetTypesFromParentClass(Assembly assembly, Type parentClass)
    {
        List<Type> allTypes = new List<Type>();
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types;
        }

        foreach (Type type in types.Where(t => t != null))
        {
            if (type.IsSubclassOf(parentClass))
                allTypes.Add(type);
        }

        return allTypes;
    }


    public static List<Type> GetTypesFromParentClass(List<Assembly> assemblies, Type parentClass)
    {
        List<Type> allTypes = new List<Type>();

        foreach (Assembly assembly in assemblies)
            allTypes.AddRange(GetTypesFromParentClass(assembly, parentClass));

        return allTypes;
    }

    public static List<Type> GetTypesFromInterface(List<Assembly> assemblies, string interfaceName)
    {
        List<Type> allTypes = new List<Type>();

        foreach (Assembly assembly in assemblies)
            allTypes.AddRange(GetTypesFromInterface(assembly, interfaceName));

        return allTypes;
    }

    public static List<Type> GetTypesFromInterface(Assembly assembly, string interfaceName)
    {
        List<Type> allTypes = new List<Type>();
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types;
        }

        foreach (Type type in types.Where(t => t != null))
        {
            if (type.GetInterface(interfaceName) != null)
                allTypes.Add(type);
        }

        return allTypes;
    }
}
