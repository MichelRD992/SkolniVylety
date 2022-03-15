using System;
using System.Collections.Generic;
using System.Text;

namespace SkolniVylety
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SkrytVeFormulariAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Property)]
    public class ReferenceAttribute : Attribute
    {
        public Type Table { get; set; }
        public ReferenceAttribute(Type table) => Table = table;
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class PopisekAttribute : Attribute
    {
        public string Text { get; set; }
        public PopisekAttribute(string text) => Text = text;
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class EnumAttribute : Attribute { }
}
