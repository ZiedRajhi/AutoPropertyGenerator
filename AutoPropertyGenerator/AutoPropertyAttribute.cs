using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPropertyGenerator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoPropertyAttribute : Attribute
    {
    }
}
