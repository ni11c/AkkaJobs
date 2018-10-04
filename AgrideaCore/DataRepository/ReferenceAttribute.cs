using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agridea.DataRepository
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ReferenceAttribute : Attribute
    {
    }
}