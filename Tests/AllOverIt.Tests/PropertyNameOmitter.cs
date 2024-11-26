using AutoFixture.Kernel;
using System.Reflection;

namespace AllOverIt.Tests
{
    internal sealed class PropertyNameOmitter : ISpecimenBuilder
    {
        private readonly IEnumerable<string> names;

        internal PropertyNameOmitter(params string[] names)
        {
            this.names = names;
        }

        public object Create(object request, ISpecimenContext context)
        {
            var propInfo = request as PropertyInfo;

            if (propInfo is not null && names.Contains(propInfo.Name))
            {
                return new OmitSpecimen();
            }

            return new NoSpecimen();
        }
    }
}