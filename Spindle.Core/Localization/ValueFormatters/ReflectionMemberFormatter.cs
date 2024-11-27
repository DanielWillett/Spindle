using DanielWillett.ReflectionTools;
using DanielWillett.ReflectionTools.Formatting;
using System;
using System.Reflection;

namespace Spindle.Localization.ValueFormatters;
public class ReflectionMemberFormatter :
    IValueStringConverter<MemberInfo>,
    IValueStringConverter<Type>,
    IValueStringConverter<MethodBase>,
    IValueStringConverter<FieldInfo>,
    IValueStringConverter<PropertyInfo>,
    IValueStringConverter<EventInfo>,
    IValueStringConverter<ParameterInfo>,
    IValueStringConverter<IMemberDefinition>,
    IValueStringConverter<IVariable>
{
    public string Format(IValueStringConvertService formatter, MemberInfo value, in ValueFormatParameters parameters)
    {
        return value switch
        {
            Type t => Accessor.Formatter.Format(t),
            MethodBase mtd => Accessor.Formatter.Format(mtd),
            FieldInfo fld => Accessor.Formatter.Format(fld),
            PropertyInfo prop => Accessor.Formatter.Format(prop),
            EventInfo ev => Accessor.Formatter.Format(ev),
            _ => value.ToString()
        };
    }

    public string Format(IValueStringConvertService formatter, Type value, in ValueFormatParameters parameters)
    {
        return Accessor.Formatter.Format(value);
    }

    public string Format(IValueStringConvertService formatter, MethodBase value, in ValueFormatParameters parameters)
    {
        return Accessor.Formatter.Format(value);
    }

    public string Format(IValueStringConvertService formatter, FieldInfo value, in ValueFormatParameters parameters)
    {
        return Accessor.Formatter.Format(value);
    }

    public string Format(IValueStringConvertService formatter, PropertyInfo value, in ValueFormatParameters parameters)
    {
        return Accessor.Formatter.Format(value);
    }

    public string Format(IValueStringConvertService formatter, EventInfo value, in ValueFormatParameters parameters)
    {
        return Accessor.Formatter.Format(value);
    }

    public string Format(IValueStringConvertService formatter, ParameterInfo value, in ValueFormatParameters parameters)
    {
        return Accessor.Formatter.Format(value);
    }

    public string Format(IValueStringConvertService formatter, IMemberDefinition value, in ValueFormatParameters parameters)
    {
        return value.Format(Accessor.Formatter);
    }

    public string Format(IValueStringConvertService formatter, IVariable value, in ValueFormatParameters parameters)
    {
        return value.Format(Accessor.Formatter);
    }

    public string Format(IValueStringConvertService formatter, object value, in ValueFormatParameters parameters)
    {
        return value switch
        {
            IMemberDefinition def => def.Format(Accessor.Formatter),
            IVariable var => var.Format(Accessor.Formatter),
            MemberInfo mem => Format(formatter, mem, in parameters),
            ParameterInfo param => Format(formatter, param, in parameters),
            _ => value.ToString()
        };
    }
}
