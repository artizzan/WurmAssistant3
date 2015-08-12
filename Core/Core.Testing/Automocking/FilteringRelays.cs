using System;
using System.Collections.Generic;

namespace AldursLab.Deprec.Core.Testing.Automocking
{
    public class FilteringRelays : DefaultEngineParts
    {
        private readonly Func<ISpecimenBuilder, bool> spec;

        public FilteringRelays(Func<ISpecimenBuilder, bool> specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException("specification");
            }

            this.spec = specification;
        }

        public override IEnumerator<ISpecimenBuilder> GetEnumerator()
        {
            var enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (this.spec(enumerator.Current))
                {
                    yield return enumerator.Current;
                }
            }
        }
    }
}
