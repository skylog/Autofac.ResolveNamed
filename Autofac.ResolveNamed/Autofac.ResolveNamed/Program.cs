using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.ResolveNamed
{
    class Program
    {
        static void Main(string[] args)
        {
            AutofacModule();
        }

        private static void AutofacModule()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Printer>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
            builder.RegisterType<Rewriter>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
            builder.RegisterType<ImgRewriter>()
                   .AsImplementedInterfaces()
                   .SingleInstance();

            builder.RegisterType<Rewriter>().Named<IRewriter>("rewriter");
            builder.RegisterType<ImgRewriter>().Named<IRewriter>("fakeRewriter");

            builder.Register(c => new Service1(new Printer(c.ResolveNamed<IRewriter>("rewriter"))))
                .As<IService1>();
            builder.Register(c => new Service2(new Printer(c.ResolveNamed<IRewriter>("fakeRewriter"))))
                .As<IService2>();

            var container = builder.Build();

            var s1 = container.Resolve<IService1>();//Contains rewriter
            var s2 = container.Resolve<IService2>();//Contains fakeRewriter
        }
    }

    public class Service1 : IService1
    {
        public Service1(IPrinter somePrinter)
        {

            somePrinter.Print();
        }
    }

    public class Service2 : IService2
    {
        public Service2(IPrinter somePrinter)
        {
            somePrinter.Print();
        }
    }

    public interface IService1
    {
    }

    public interface IService2
    {
    }

    public interface IPrinter
    {
        void Print();
    }

    public class Printer : IPrinter
    {
        readonly IRewriter rewriter;
        public Printer(IRewriter rewriter)
        {
            this.rewriter = rewriter;
        }

        public void Print()
        {
            rewriter.Do();
        }
    }

    public interface IRewriter
    {
        void Do();
    }

    public class Rewriter : IRewriter
    {
        public void Do()
        {
            Console.WriteLine("Rewriter");
        }
    }

    public class ImgRewriter : IRewriter
    {
        public void Do()
        {
            Console.WriteLine("ImgRewriter");
        }
    }
}
