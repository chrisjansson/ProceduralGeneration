using FluentAssertions;
using NUnit.Framework;

namespace CjClutter.Commons.Reflection
{
    [TestFixture]
    public class PropertyHelperTests
    {
        [Test]
        public void Returns_correct_name_for_property_getter()
        {
            var propertyName = PropertyHelper.GetPropertyName((PropertyHelperClass x) => x.TestProperty);

            propertyName.Should().Be("TestProperty");
        }

        [Test]
        public void Returns_correct_name_for_static_property()
        {
            var propertyName = PropertyHelper.GetPropertyName(() => PropertyHelperClass.StaticTestProperty);

            propertyName.Should().Be("StaticTestProperty");
        }

        private abstract class PropertyHelperClass
        {
            public static int StaticTestProperty { get { return 0; } }
            public string TestProperty { get; set; }
        }
    }
}