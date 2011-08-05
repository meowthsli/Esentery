namespace Meowth.Esentery.Querying
{
    /// <summary> Complex predicate. Has sub-predicates </summary>
    public abstract class ComplexPredicate : Predicate
    {
        /// <summary> Complex predicate base </summary>
        protected ComplexPredicate(params Predicate[] subs)
        {
            Subpredicates = (Predicate[]) subs.Clone();
        }
        
        protected readonly Predicate[] Subpredicates;
    }
}