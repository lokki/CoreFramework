using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Reflection;

namespace Framework.AspNetCore
{
    public class EntityModelBinderProvider : IModelBinderProvider
    {
        private readonly Type _entityType;
        private readonly IModelBinder _entityModelBinder;

        public EntityModelBinderProvider(Type entityType, IModelBinder entityModelBinder)
        {
            _entityType = entityType;
            _entityModelBinder = entityModelBinder;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return _entityType.IsAssignableFrom(context.Metadata.ModelType) ? _entityModelBinder : null;
        }
    }
}
