namespace Meowth.Esentery.Querying
{
    /// <summary> Complex predicate </summary>
    public abstract class ComplexPredicate : Predicate
    {
        protected ComplexPredicate(params Predicate[] subs)
        {
            Subpredicates = (Predicate[]) subs.Clone();
        }
        
        protected readonly Predicate[] Subpredicates;
    }
}