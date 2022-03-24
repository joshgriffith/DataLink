using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataLink.Core.ChangeTracking;
using DataLink.Core.Extensions;
using DataLink.Core.Filters;
using DataLink.Core.Interceptors;
using DataLink.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataLink.Core.Configuration {
    public class DataHubConfiguration {
        private readonly IServiceCollection _services;

        public DataHubConfiguration()
            : this(new ServiceCollection()) {
        }

        public DataHubConfiguration(IServiceCollection services) {
            _services = services;
        }

        public static DataHubConfiguration Default() {
            var configuration = new DataHubConfiguration();
            configuration.Use<DataHub>();
            configuration.Use<SnapshotChangeTracker>();
            configuration.Use<InterceptionDispatcher>();
            return configuration;
        }
        
        public DataHubConfiguration Use<T>(T singleton) where T : class {
            Register(singleton);
            return this;
        }

        public DataHubConfiguration Use<T>() where T : class {
            Register<T>();
            return this;
        }

        public DataHubConfiguration Use<T>(Func<IQueryable<T>> factory) where T : class {
            Register(typeof(DefaultRepositoryFactory<T>), _ => new DefaultRepositoryFactory<T>(factory));
            return this;
        }

        public DataHubConfiguration Use<T>(Func<T> factory) where T : class {
            Register(typeof(T), _ => factory());
            return this;
        }

        public DataHubConfiguration Filter<T>(Expression<Func<T, bool>> expression, QueryFilterTypes type = QueryFilterTypes.Inclusive) {
            Register(typeof(DefaultQueryFilter<T>), _ => new DefaultQueryFilter<T>(expression, type));
            return this;
        }
        
        private void Register<T>() where T : class {
            Register(typeof(T), null);
        }

        private void Register(Type type, Func<IServiceProvider, object> factory) {
            
            if (factory == null)
                _services.AddScoped(type);
            else
                _services.AddScoped(type, factory);

            foreach (var each in GetTypes(type))
                _services.AddScoped(each, provider => provider.GetRequiredService(type));
        }

        private void Register<T>(T instance) where T : class {
            
            _services.AddSingleton(instance);
            
            //foreach (var each in GetTypes(instance.GetType()))
            //    _services.AddSingleton(each, provider => provider.GetRequiredService(typeof(T)));
        }

        internal IServiceProvider BuildServiceProvider() {
            return _services.BuildServiceProvider();
        }

        private static IEnumerable<Type> GetTypes(Type type) {
            return type.GetBaseTypes().Where(each => each.Namespace != null && !each.Namespace.StartsWith(nameof(System)));
        }
    }
}