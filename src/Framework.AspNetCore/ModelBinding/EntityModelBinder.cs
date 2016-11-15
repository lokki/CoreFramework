using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Framework.AspNetCore
{
    public abstract class EntityModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var original = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (original != ValueProviderResult.None)
            {
                var originalValue = original.FirstValue;
                long id;
                if (long.TryParse(originalValue, out id))
                {
                    var entity = await GetEntity(id, bindingContext.HttpContext.RequestServices, bindingContext.ModelType);

                    bindingContext.Result = ModelBindingResult.Success(entity);
                }
            }
        }

        protected abstract Task<object> GetEntity(long id, IServiceProvider requestServices, Type modelType);
    }
}
