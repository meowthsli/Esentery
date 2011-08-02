using Meowth.Esentery.Core;

namespace Meowth.Esentery
{
    internal interface ISessionObject
    {
        Session CurrentSession { get; }
    }
}