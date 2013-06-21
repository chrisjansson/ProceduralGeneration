using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace ObjLoader.Test
{
    public class Container
    {
        public T GetInstance<T>()
        {
            var type = typeof(T);

            return (T) GetInstance(type);
        }

        private object GetInstance(Type requestType)
        {
            if (!_typeRegistry.ContainsKey(requestType)) throw new Exception();

            var targetType = _typeRegistry[requestType];

            var constructor = targetType.GetConstructors().Single();
            var parameterInfos = constructor.GetParameters();
            var parameters = parameterInfos.Select(x => x.ParameterType)
                          .Select(GetInstance)
                          .ToArray();

            return constructor.Invoke(parameters);         
        }

        public ForExpression<T> For<T>()
        {
            return new ForExpression<T>(this);
        }

        public class ForExpression<TRequestType>
        {
            private readonly Container _container;

            public ForExpression(Container container)
            {
                _container = container;
            }

            public void Use<TTArgetType>() where TTArgetType : TRequestType
            {
                var requestType = typeof(TRequestType);
                var targetType = typeof(TTArgetType);

                _container.Register(requestType, targetType);
            }
        }

        private readonly IDictionary<Type, Type> _typeRegistry = new Dictionary<Type, Type>();
        private void Register(Type requestType, Type targetType)
        {
            _typeRegistry.Add(requestType, targetType);
        }
    }

    [TestFixture]
    public class ContainerTests
    {
        private Container _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Container();
        }

        [Test]
        public void Can_not_instantiate_types_that_are_not_registered()
        {
            TestDelegate act = () => _sut.GetInstance<object>();

            Assert.That(act, Throws.TypeOf<Exception>());
        }

        [Test]
        public void Instantiates_registered_object()
        {
            _sut.For<object>().Use<object>();

            var instance = _sut.GetInstance<object>();

            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void Instantiates_different_objects_each_time()
        {
            _sut.For<object>().Use<object>();

            var first = _sut.GetInstance<object>();
            var second = _sut.GetInstance<object>();

            Assert.That(first, Is.Not.SameAs(second));
        }

        [Test]
        public void Instantiates_type_with_non_parameter_less_constructor()
        {
            _sut.For<object>().Use<object>();
            _sut.For<ObjectWithParameterInConstructor>().Use<ObjectWithParameterInConstructor>();

            var instance = _sut.GetInstance<ObjectWithParameterInConstructor>();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.ObjectDependency, Is.Not.Null & Is.InstanceOf<object>());
        }

        [Test]
        public void Instantiates_matches_type_instantiation_on_inheritance()
        {
            _sut.For<object>().Use<object>();
            _sut.For<BaseType>().Use<ObjectWithParameterInConstructor>();

            var instance = _sut.GetInstance<BaseType>();

            Assert.That(instance, Is.InstanceOf<ObjectWithParameterInConstructor>());
        }

        [Test]
        public void Instantiation_matches_on_interface()
        {
            _sut.For<object>().Use<object>();
            _sut.For<ISomeInterface>().Use<ObjectWithParameterInConstructor>();

            var instance = _sut.GetInstance<ISomeInterface>();

            Assert.That(instance, Is.InstanceOf<ObjectWithParameterInConstructor>());
        }
    }

    public interface ISomeInterface {}

    public class BaseType {}

    public class ObjectWithParameterInConstructor : BaseType, ISomeInterface
    {
        public readonly object ObjectDependency;

        public ObjectWithParameterInConstructor(object obj)
        {
            ObjectDependency = obj;
        }
    }
}