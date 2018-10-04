using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agridea.DataRepository
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class MandatoryAttribute : Attribute
    {
    }
}