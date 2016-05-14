using System;

namespace Rabbitual
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AdminUrlAttribute : Attribute
    {
        /// <summary>
        /// Url to custom UI for this agent in the admin interface.
        /// Relative url to Rabbitual.Web root.
        /// </summary>
        public string Url { get; set; }

        public AdminUrlAttribute(string url)
        {
            Url = url;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class IconAttribute : Attribute
    {
        public string FontAwesome { get; set; }

        public IconAttribute(string fontAwesome)
        {
            FontAwesome = fontAwesome;
        }
    }
}