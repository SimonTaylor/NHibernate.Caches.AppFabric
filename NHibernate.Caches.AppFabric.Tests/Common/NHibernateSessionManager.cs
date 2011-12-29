using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.Caches.AppFabric.Tests.Common
{
    public static class NHibernateSessionManager
    {
        #region Constants

        private const string ConnectionString = "Data Source=TestData.sdf;Persist Security Info=False;";

        #endregion

        #region Member variables

        private static ISessionFactory _sessionFactory;

        #endregion

        #region Methods

        public static Configuration Initialize()
        {
            ModelMapper mapper = new ModelMapper();

            mapper.AddMappings(Assembly.GetAssembly(typeof(NHibernateSessionManager)).GetExportedTypes());

            Configuration cfg = new Configuration();

            cfg.DataBaseIntegration(c =>
            {
                c.Driver<SqlServerCeDriver>();
                c.Dialect<MsSqlCe40Dialect>();

                c.LogSqlInConsole = true;
                c.LogFormattedSql = true;

                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                c.ConnectionString   = ConnectionString;
            });
            cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            cfg.CurrentSessionContext<ThreadStaticSessionContext>();

            // Set up second level caching
            cfg.Cache(props =>
            {
                props.Provider<AppFabricProvider>();
                props.UseQueryCache = true;
            });

            // (Re-)create the database.
            new SchemaExport(cfg).Execute(false, true, false);

            _sessionFactory = cfg.BuildSessionFactory();

            return cfg;
        }

        public static void OpenSession()
        {
            if (!CurrentSessionContext.HasBind(_sessionFactory))
            {
                ISession session = _sessionFactory.OpenSession();
                session.FlushMode = FlushMode.Commit;

                CurrentSessionContext.Bind(session);
            }
        }

        public static void CloseSession()
        {
            ISession     session     = CurrentSessionContext.Unbind(_sessionFactory);
            ITransaction transaction = session.Transaction;

            if (transaction.IsActive)
                transaction.Dispose();

            session.Dispose();
        }

        public static void BeginTransaction()
        {
            if (!CurrentSession.Transaction.IsActive)
                CurrentSession.BeginTransaction();
        }

        public static void CommitTransaction()
        {
            if (CurrentSession.Transaction.IsActive)
                CurrentSession.Transaction.Commit();
        }

        public static void RollbackTransaction()
        {
            if (CurrentSession.Transaction.IsActive)
                CurrentSession.Transaction.Rollback();
        }

        #endregion

        #region Properties

        public static ISession CurrentSession
        {
            get { return _sessionFactory.GetCurrentSession(); }
        }

        #endregion
    }
}
