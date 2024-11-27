using System;

namespace Spindle.Localization;
public interface IArgumentAddon
{
    string DisplayName { get; }
    string ApplyAddon(IValueStringConvertService formatter, string text, TypedReference value, in ValueFormatParameters args);
}