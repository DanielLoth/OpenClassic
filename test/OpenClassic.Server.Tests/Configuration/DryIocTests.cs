using DryIoc;
using System.Linq;
using Xunit;

namespace OpenClassic.Server.Tests.Configuration
{
    public class DryIocTests
    {
        [Fact]
        public void SameSingletonReturnedOnSuccessiveResolve()
        {
            var container = new Container();

            container.Register<IOuter, Outer>(Reuse.Singleton);
            container.Register<IInner, Inner>();

            var outer1 = container.Resolve<IOuter>();
            var outer2 = container.Resolve<IOuter>();

            Assert.Same(outer1, outer2);
        }

        [Fact]
        public void DifferentTransientReturnedOnSuccessiveResolve()
        {
            var container = new Container();

            container.Register<IOuter, Outer>(Reuse.Transient);
            container.Register<IInner, Inner>();

            var outer1 = container.Resolve<IOuter>();
            var outer2 = container.Resolve<IOuter>();

            Assert.NotSame(outer1, outer2);
        }

        [Fact]
        public void DifferentOuterHaveDifferentInnerForResolutionScope()
        {
            var container = new Container();

            container.Register<IInner, Inner>(Reuse.InResolutionScopeOf<IOuter>());
            container.Register<IOuter, Outer>(Reuse.Transient);

            var outer1 = container.Resolve<IOuter>();
            var outer2 = container.Resolve<IOuter>();

            Assert.NotSame(outer1, outer2);
            Assert.NotSame(outer1.Inner, outer2.Inner);
        }

        [Fact]
        public void SameOuterShareSameInnerForResolutionScope()
        {
            var container = new Container();

            container.Register<IInner, Inner>(Reuse.InResolutionScopeOf<IOuter>());
            container.Register<IOuter, Outer>(Reuse.Singleton);

            var outer1 = container.Resolve<IOuter>();
            var outer2 = container.Resolve<IOuter>();

            Assert.Same(outer1, outer2);
            Assert.Same(outer1.Inner, outer2.Inner);
        }

        [Fact]
        public void TestObjectSingleDecoration()
        {
            var container = new Container();

            container.Register<IOuter, Outer>();
            container.Register<IInner, Inner>();

            container.Register<IOuter, OuterDecoratorA>(setup: Setup.Decorator);

            var outer = container.Resolve<IOuter>();

            Assert.IsType<OuterDecoratorA>(outer);
        }

        [Fact]
        public void TestObjectMultipleDecoration()
        {
            var container = new Container();

            container.Register<IOuter, Outer>();
            container.Register<IInner, Inner>();

            container.Register<IOuter, OuterDecoratorA>(setup: Setup.Decorator);
            container.Register<IOuter, OuterDecoratorB>(setup: Setup.Decorator);

            var outer = container.Resolve<IOuter>();

            // The outermost decorator is that decorator which was registered last.
            Assert.IsType<OuterDecoratorB>(outer);

            var outerDecoratorA = outer as OuterDecoratorB;
            Assert.NotNull(outerDecoratorA);
            Assert.IsType<OuterDecoratorA>(outerDecoratorA.deleg);
        }

        [Fact]
        public void TestMultipleResolution()
        {
            var container = new Container();

            container.Register<IInner, Inner>();
            container.Register<IOuter, Outer>();
            container.Register<IOuter, Outer2>();

            var outerObjects = container.ResolveMany<IOuter>();

            Assert.NotNull(outerObjects);
            Assert.Equal(2, outerObjects.Count());

            Assert.NotNull(outerObjects.FirstOrDefault(x => x is Outer));
            Assert.NotNull(outerObjects.FirstOrDefault(x => x is Outer2));
        }

        #region DI dummy classes

        internal interface IInner { }
        internal class Inner : IInner { }

        internal interface IOuter
        {
            IInner Inner { get; }
        }

        internal class Outer : IOuter
        {
            readonly IInner inner;

            public Outer(IInner inner)
            {
                this.inner = inner;
            }

            public IInner Inner => inner;
        }

        internal class Outer2 : Outer
        {
            public Outer2(IInner inner) : base(inner)
            {
            }
        }

        internal class OuterDecoratorA : IOuter
        {
            public readonly IOuter deleg;

            public OuterDecoratorA(IOuter deleg)
            {
                this.deleg = deleg;
            }

            public IInner Inner => deleg.Inner;
        }

        internal class OuterDecoratorB : IOuter
        {
            public readonly IOuter deleg;

            public OuterDecoratorB(IOuter deleg)
            {
                this.deleg = deleg;
            }

            public IInner Inner => deleg.Inner;
        }

        #endregion
    }
}
