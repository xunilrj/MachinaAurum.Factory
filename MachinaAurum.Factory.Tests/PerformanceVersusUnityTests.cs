using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Autofac;

namespace MachinaAurum.Factory.Tests
{
    [TestClass]
    public class PerformanceVersusUnityTests
    {
        [TestMethod]
        public void FactoryVersusUnity()
        {
            var factory = new MainFactory();

            var stopWatchFactory = Stopwatch.StartNew();

            using (var context = factory.CreateContext())
            {
                for (int i = 0; i < 1000000; i++)
                {
                    var instance = context.Resolve<A>();
                    instance.Index = i;

                    Assert.IsNotNull(instance);
                    Assert.IsNotNull(instance.B);
                    Assert.IsNotNull(instance.B.C);
                    Assert.IsNotNull(instance.C);

                    Assert.AreNotSame(instance.B.C, instance.C);
                }
            }

            stopWatchFactory.Stop();

            var unity = new UnityContainer();

            var stopWatchUnity = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                var instance = unity.Resolve<A>();
                instance.Index = i;

                Assert.IsNotNull(instance);
                Assert.IsNotNull(instance.B);
                Assert.IsNotNull(instance.B.C);
                Assert.IsNotNull(instance.C);

                Assert.AreNotSame(instance.B.C, instance.C);
            }

            stopWatchUnity.Stop();

            Console.WriteLine(string.Format("Factory: {0}", stopWatchFactory.Elapsed));
            Console.WriteLine(string.Format("Unity: {0}", stopWatchUnity.Elapsed));
        }

        [TestMethod]
        public void FactoryVersusAutofac()
        {
            var factory = new MainFactory();

            var stopWatchFactory = Stopwatch.StartNew();

            using (var context = factory.CreateContext())
            {
                for (int i = 0; i < 1000000; i++)
                {
                    var instance = context.Resolve<A>();
                    instance.Index = i;

                    Assert.IsNotNull(instance);
                    Assert.IsNotNull(instance.B);
                    Assert.IsNotNull(instance.B.C);
                    Assert.IsNotNull(instance.C);

                    Assert.AreNotSame(instance.B.C, instance.C);
                }
            }

            stopWatchFactory.Stop();

            var builder = new ContainerBuilder();
            builder.RegisterType<A>();
            builder.RegisterType<B>();
            builder.RegisterType<C>();

            var autoFacContext = builder.Build();

            var stopWatchAutofac = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                var instance = autoFacContext.Resolve<A>();
                instance.Index = i;

                Assert.IsNotNull(instance);
                Assert.IsNotNull(instance.B);
                Assert.IsNotNull(instance.B.C);
                Assert.IsNotNull(instance.C);

                Assert.AreNotSame(instance.B.C, instance.C);
            }

            stopWatchAutofac.Stop();

            Console.WriteLine(string.Format("Factory: {0}", stopWatchFactory.Elapsed));
            Console.WriteLine(string.Format("Autofac: {0}", stopWatchAutofac.Elapsed));
        }
    }
}
