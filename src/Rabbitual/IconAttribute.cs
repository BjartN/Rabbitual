using System;

namespace Rabbitual
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IconAttribute:Attribute
    {
        public string FontAwesome { get; set; }

        public IconAttribute(string fontAwesome)
        {
            FontAwesome = fontAwesome;
        }
    }
}