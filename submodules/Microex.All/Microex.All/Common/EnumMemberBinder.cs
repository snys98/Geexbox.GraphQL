//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Json;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
//using Newtonsoft.Json;

//namespace Microex.All.Common
//{
//    /// <summary>
//    /// url内部的参数不能通过EnumMember正常序列化,这个binder用来处理这种情况
//    /// </summary>
//    public class EnumMemberBinder : IModelBinder
//    {
//        public async Task BindModelAsync(ModelBindingContext bindingContext)
//        {
//            if (bindingContext == null)
//            {
//                throw new ArgumentNullException(nameof(bindingContext));
//            }

//            // Specify a default argument name if none is set by ModelBinderAttribute
//            var modelName = bindingContext.ModelName;
//            if (string.IsNullOrEmpty(modelName))
//            {
//                modelName = "UrlDecode";
//            }


//            var rawResult = bindingContext.ValueProvider.GetValue(modelName);
//            if (rawResult == ValueProviderResult.None)
//            {
//                return;
//            }

//            var value = rawResult.FirstValue;

//            // Check if the argument value is null or empty
//            if (string.IsNullOrEmpty(value))
//            {
//                return;
//            }
//            var enumType = bindingContext.ModelType.IsEnum? bindingContext.ModelType: bindingContext.ModelType.GenericTypeArguments[0];
//            foreach (var name in Enum.GetNames(enumType))
//            {
//                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).FirstOrDefault();
//                if (enumMemberAttribute == null)
//                {
//                    bindingContext.Result = ModelBindingResult.Success(Enum.Parse(enumType, name));
//                }
//                else if (enumMemberAttribute.Value == value)
//                {
//                    bindingContext.Result = ModelBindingResult.Success(Enum.Parse(enumType, name));
//                    return;
//                }
//            }
//            //throw exception or whatever handling you want or
//            throw new Exception($"unsupported enum value [{value}] of type [{enumType}]");
//        }
//    }

//    public class EnumMemberBinderProvider : IModelBinderProvider
//    {
//        public IModelBinder GetBinder(ModelBinderProviderContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }
//            // 如果是枚举或者nullable的枚举
//            if ((context.Metadata.ModelType.IsEnum || (context.Metadata.ModelType.IsGenericType&& context.Metadata.ModelType.GenericTypeArguments[0].IsEnum)))
//            {
//                return new BinderTypeModelBinder(typeof(EnumMemberBinder));
//            }

//            return null;
//        }
//    }
//}
