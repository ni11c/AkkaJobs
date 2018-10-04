using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace Agridea.Validation
{
    /// <summary>
    /// If FluentValidation is not an option, use this class as 
    /// a building block for validators
    /// </summary>
    [Serializable]
    public class ValidationException<TModel> : Exception
    {
        #region Members
        private readonly IList<ValidationError> validationErrors_ = new List<ValidationError>();
        #endregion

        #region Initialization
        public ValidationException() : base() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion

        #region Services
        public void AddError(string message)
        {
            validationErrors_.Add(new ValidationError { Property = This, Message = message });
        }
        public void AddError<TProperty>(Expression<Func<TModel, TProperty>> property, string message)
        {
            validationErrors_.Add(new ValidationError { Property = property, Message = message });
        }
        public void ValidateAndThrow()
        {
            if (validationErrors_.Count > 0) throw this;
        }
        public void CopyTo(ModelStateDictionary modelState)
        {
            foreach (var propertyError in validationErrors_)
            {
                string key = ExpressionHelper.GetExpressionText(propertyError.Property);
                modelState.AddModelError(key, propertyError.Message);
            }
        }

        public void CopyToWithIndex(ModelStateDictionary modelState, int index)
        {
            foreach (var propertyError in validationErrors_)
            {
                string key = ExpressionHelper.GetExpressionText(propertyError.Property);
                modelState.AddModelError("[" + index.ToString() + "]." + key, propertyError.Message);
            }
        }

        #endregion

        #region Helpers
        [Serializable]
        private class ValidationError
        {
            public LambdaExpression Property { get; set; }
            public string Message { get; set; }
        }
        private readonly static Expression<Func<object, object>> This = x => x;
        #endregion
    }
}
