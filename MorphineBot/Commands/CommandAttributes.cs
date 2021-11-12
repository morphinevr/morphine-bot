using System;

namespace MorphineBot.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = true)]
    public class AdminCommand : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = true)]
    public class OwnerCommand : Attribute
    {
    }
}