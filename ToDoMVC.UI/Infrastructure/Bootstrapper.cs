namespace ToDoMVC.UI
{ 
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using FluentValidation;
    using FluentValidation.Mvc;
    using Incoding.Block.IoC;
    using Incoding.Block.Logging;
    using Incoding.CQRS;
    using Incoding.Data;
    using Incoding.EventBroker;
    using Incoding.Extensions;
    using Incoding.MvcContrib;
    using NHibernate.Context;
    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Indexes;
    using Raven.Client.Listeners;

    public static class Bootstrapper
    {

        public class NoStaleQueriesListener : IDocumentQueryListener
        {
            #region Implementation of IDocumentQueryListener

            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
                queryCustomization.WaitForNonStaleResults();
            }

            #endregion
        }


      public  const string nhibernateInstance = "Nhibernate";
      public  const string ravenDbInstance = "RavenDb";

        public static void Start()
        {
            LoggingFactory.Instance.Initialize(logging =>
                                                   {
                                                       string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                                                       logging.WithPolicy(policy => policy.For(LogType.Debug).Use(FileLogger.WithAtOnceReplace(path, () => "Debug_{0}.txt".F(DateTime.Now.ToString("yyyyMMdd")))));
                                                   });

            IoCFactory.Instance.Initialize(init => init.WithProvider(new StructureMapIoCProvider(registry =>
                                                                                                     {
                                                                                                         registry.For<IDispatcher>().Singleton().Use<DefaultDispatcher>();
                                                                                                         registry.For<IEventBroker>().Singleton().Use<DefaultEventBroker>();
                                                                                                         registry.For<ITemplateFactory>().Singleton().Use<TemplateHandlebarsFactory>();

                                                                                                     /*    var configure = Fluently
                                                                                                                 .Configure()
                                                                                                                 .Database(MsSqlConfiguration.MsSql2008.ConnectionString(ConfigurationManager.ConnectionStrings["Main"].ConnectionString))
                                                                                                                 .Mappings(configuration => configuration.FluentMappings.AddFromAssembly(typeof(Bootstrapper).Assembly))
                                                                                                                 .CurrentSessionContext<ThreadStaticSessionContext>();
                                                                                                         
                                                                                                         registry.For<IManagerDataBase>().Singleton().Use(() => new NhibernateManagerDataBase(configure)).Named(nhibernateInstance);
                                                                                                         registry.For<INhibernateSessionFactory>().Singleton().Use(() => new NhibernateSessionFactory(configure)).Named(nhibernateInstance);
                                                                                                         registry.For<IUnitOfWorkFactory>().Singleton().Use<NhibernateUnitOfWorkFactory>().Named(nhibernateInstance);
                                                                                                         registry.For<IRepository>().Use<NhibernateRepository>().Named(nhibernateInstance);*/

                                                                                                         registry.For<IRavenDbSessionFactory>().Singleton().Use(() =>
                                                                                                                                                                    {
                                                                                                                                                                        var documentStore = new DocumentStore
                                                                                                                                                                                                {
                                                                                                                                                                                                        DefaultDatabase = "TodoMvc",
                                                                                                                                                                                                        Url = "http://localhost:8080/",
                                                                                                                                                                                                };
                                                                                                                                                                        documentStore.Conventions.AllowQueriesOnId = true;
                                                                                                                                                                        documentStore.Conventions.MaxNumberOfRequestsPerSession = 1000;
                                                                                                                                                                        documentStore.Initialize();
                                                                                                                                                                        documentStore.RegisterListener(new NoStaleQueriesListener());
                                                                                                                                                                        IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), documentStore);

                                                                                                                                                                        return new RavenDbSessionFactory(documentStore);
                                                                                                                                                                    }).Named(ravenDbInstance);
                                                                                                         registry.For<IUnitOfWorkFactory>().Singleton().Use<RavenDbUnitOfWorkFactory>().Named(ravenDbInstance);
                                                                                                         registry.For<IRepository>().Use<RavenDbRepository>().Named(ravenDbInstance);

                                                                                                         registry.Scan(r =>
                                                                                                                           {
                                                                                                                               
                                                                                                                               r.WithDefaultConventions();

                                                                                                                               r.ConnectImplementationsToTypesClosing(typeof(AbstractValidator<>));
                                                                                                                               r.ConnectImplementationsToTypesClosing(typeof(IEventSubscriber<>));
                                                                                                                               r.AddAllTypesOf<ISetUp>();
                                                                                                                           });
                                                                                                     })));

            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new IncValidatorFactory()));
            FluentValidationModelValidatorProvider.Configure();

            foreach (var setUp in IoCFactory.Instance.ResolveAll<ISetUp>().OrderBy(r => r.GetOrder()))
                setUp.Execute();

            var ajaxDef = JqueryAjaxOptions.Default;
            ajaxDef.Cache = false; // disabled cache as default
        }
    }

}