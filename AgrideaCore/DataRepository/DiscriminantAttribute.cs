using System;
using System.ComponentModel.DataAnnotations;

namespace Agridea.DataRepository
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DiscriminantAttribute : RequiredAttribute
    {
    }
}
