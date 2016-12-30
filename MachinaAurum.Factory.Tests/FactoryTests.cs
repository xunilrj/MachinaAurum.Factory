using MachinaAurum.Factory.LifetimeManagers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MachinaAurum.Factory.Tests
{
    public interface IA
    {
    }

    public class TestA : IA
    {
    }

    public class A : IA, IDisposable
    {
        public B B { get; set; }
        public C C { get; set; }

        public int Index { get; set; }

        public A(B b, C c)
        {
            B = b;
            C = c;
        }

        public event EventHandler OnDispose;

        public void Dispose()
        {
            if (OnDispose != null)
            {
                OnDispose(this, EventArgs.Empty);
            }
        }
    }

    public class B
    {
        public C C { get; set; }

        public B(C c)
        {
            C = c;
        }
    }

    public class C : IDisposable
    {
        public event EventHandler OnDispose;

        public void Dispose()
        {
            if (OnDispose != null)
            {
                OnDispose(this, EventArgs.Empty);
            }
        }
    }

    public class D
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public D(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }

    public interface IMessage
    {
        void Send();
    }

    public class MessageAttribute : Attribute
    {
        public string Urgency { get; set; }
    }

    [Message(Urgency = "low")]
    public class Email : IMessage
    {
        public event EventHandler Sent;

        public void Send()
        {
            Sent(this, EventArgs.Empty);
        }
    }

    [Message(Urgency = "high")]
    public class Sms : IMessage
    {
        public event EventHandler Sent;

        public void Send()
        {
            Sent(this, EventArgs.Empty);
        }
    }

    public class MessageSender
    {
        Func<string, IMessage> GetMessageByUrgency;

        public MessageSender(Func<string, IMessage> getMessageByUrgency)
        {
            GetMessageByUrgency = getMessageByUrgency;
        }

        public void SendUrgent()
        {
            var message = GetMessageByUrgency("high");
            message.Send();
        }
    }

    [TestClass]
    public class FactoryTests
    {
        [TestMethod]
        public void ICanCreateTypeWithDependenciesInTheConstructor()
        {
            var factory = new MainFactory();
            using (var context = factory.CreateContext())
            {
                var instance = context.Resolve<A>();

                Assert.IsNotNull(instance);
                Assert.IsNotNull(instance.B);
                Assert.IsNotNull(instance.B.C);
                Assert.IsNotNull(instance.C);

                Assert.AreNotSame(instance.B.C, instance.C);

                Assert.IsInstanceOfType(context.GetLifetime(instance), typeof(NewInstanceLifetimeManager));
                Assert.IsInstanceOfType(context.GetLifetime(instance.B), typeof(NewInstanceLifetimeManager));
                Assert.IsInstanceOfType(context.GetLifetime(instance.B.C), typeof(NewInstanceLifetimeManager));
                Assert.IsInstanceOfType(context.GetLifetime(instance.C), typeof(NewInstanceLifetimeManager));
            }
        }

        [TestMethod]
        public void ICanCreateDependenciesAsSingletons()
        {
            var factory = new MainFactory();
            factory.RegisterLifetime<C>(new FactorySingletonLifetimeManager());

            using (var context = factory.CreateContext())
            {
                var instance = context.Resolve<A>();

                Assert.IsNotNull(instance);
                Assert.IsNotNull(instance.B);
                Assert.IsNotNull(instance.B.C);
                Assert.IsNotNull(instance.C);

                Assert.AreSame(instance.B.C, instance.C);
            }
        }

        //[TestMethod]
        //public void ICanControlDependenciesLifetimeUsingContextHierarchyWithParentLifetime()
        //{
        //    bool disposedA = false;
        //    bool disposedC = false;

        //    var factory = new MainFactory();
        //    factory.RegisterAsFactoryContextSingleton<C>();

        //    using (var context1 = factory.CreateContext())
        //    {
        //        var instanceC = context1.Resolve<C>();

        //        instanceC.OnDispose += (s, e) => { disposedC = true; };

        //        using (var context2 = context1.CreateContext())
        //        {
        //            var instance = context2.Resolve<A>();
        //            instance.OnDispose += (s, e) => { disposedA = true; };

        //            Assert.IsNotNull(instance);
        //            Assert.IsNotNull(instance.B);
        //            Assert.IsNotNull(instance.B.C);
        //            Assert.IsNotNull(instance.C);

        //            Assert.AreSame(instance.B.C, instance.C);
        //            Assert.AreSame(instanceC, instance.C);
        //        }

        //        Assert.IsTrue(disposedA);
        //        Assert.IsFalse(disposedC);
        //    }

        //    Assert.IsTrue(disposedC);
        //}

        //[TestMethod]
        //public void ICanControlDependenciesLifetimeUsingContextHierarchyWithCurrentLifetime()
        //{
        //    bool disposedA = false;
        //    bool disposedC1 = false;
        //    bool disposedC2 = false;

        //    var factory = new MainFactory();
        //    factory.RegisterAsFactoryContextSingleton<C>();

        //    using (var context1 = factory.CreateContext())
        //    {
        //        var instanceC = context1.Resolve<C>();

        //        instanceC.OnDispose += (s, e) => { disposedC1 = true; };

        //        using (var context2 = context1.CreateContext())
        //        {
        //            context2.RegisterSingleton<C>(null);

        //            var instance = context2.Resolve<A>();
        //            instance.OnDispose += (s, e) => { disposedA = true; };

        //            instance.C.OnDispose += (s, e) => { disposedC2 = true; };

        //            Assert.IsNotNull(instance);
        //            Assert.IsNotNull(instance.B);
        //            Assert.IsNotNull(instance.B.C);
        //            Assert.IsNotNull(instance.C);

        //            Assert.AreSame(instance.B.C, instance.C);
        //            Assert.AreNotSame(instanceC, instance.C);
        //        }

        //        Assert.IsTrue(disposedC2);

        //        Assert.IsTrue(disposedA);
        //        Assert.IsFalse(disposedC1);
        //    }

        //    Assert.IsTrue(disposedC1);
        //}

        [TestMethod]
        public void ICanCreateFromInterfaces()
        {
            var factory = new MainFactory();

            using (var context = factory.CreateContext())
            {
                var instance = context.Resolve<IA>();

                Assert.IsNotNull(instance);
                Assert.IsInstanceOfType(instance, typeof(A));
            }
        }

        [TestMethod]
        public void IFactoryContextDisposeMustDisposeCreatedObjects()
        {
            bool disposedA = false;
            bool disposedC = false;

            var factory = new MainFactory();

            using (var context = factory.CreateContext())
            {
                var instance = context.Resolve<A>();

                instance.OnDispose += (s, e) => { disposedA = true; };
                instance.C.OnDispose += (s, e) => { disposedC = true; };
            }

            Assert.IsTrue(disposedA);
            Assert.IsTrue(disposedC);
        }

        [TestMethod]
        public void ICanChooseAnImplementationForAnInterface()
        {
            var factory = new MainFactory();
            factory.RegisterImplementation<IA, TestA>();

            using (var context = factory.CreateContext())
            {
                var instance = context.Resolve<IA>();

                Assert.IsInstanceOfType(instance, typeof(TestA));
            }
        }

        //[TestMethod]
        //public void ICanCreateClassesWithNonDependencyParameters()
        //{
        //    var factory = new MainFactory();

        //    using (var context = factory.CreateContext())
        //    {
        //        var instance = context.Resolve<D>("name", "name", "password", "password");
        //    }
        //}

        [TestMethod]
        public void ICanSearchForDependenciesMetadata()
        {
            var factory = new MainFactory();
            factory.RegisterAsFactoryContextSingleton<Email>();
            factory.RegisterAsFactoryContextSingleton<Sms>();

            using (var context = factory.CreateContext())
            {
                bool sent = false;
                var sms = context.Resolve<Sms>();
                sms.Sent += (s, e) => { sent = true; };

                var sender = context.Resolve<MessageSender>();
                sender.SendUrgent();

                Assert.IsTrue(sent);
            }
        }
    }
}
