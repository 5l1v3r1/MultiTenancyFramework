﻿using System;
using System.Net;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class ErrorMessageModel : HandleErrorInfo
    {
        public const string ErrorMessageKey = "ErrorMessageKey";
        /// <summary>
        /// The (MVC) area name
        /// </summary>
        public string AreaName { get; set; }
        public string ErrorMessage { get; private set; }
        public string StackTrace { get; private set; }
        /// <summary>
        /// Gets the Url where the error originated from
        /// </summary>
        public string FromUrl { get; set; }
        /// <summary>
        /// Gets or sets the response code. Default is 400 (Bad Request)
        /// </summary>
        /// <value>
        /// The response code.
        /// </value>
        public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.BadRequest;
        /// <summary>
        /// typeof(Exception): The exception's type.
        /// </summary>
        public Type ExceptionType { get; private set; }
        /// <summary>
        /// Error type
        /// </summary>
        public ExceptionType? ErrorType { get; set; }
        /// <summary>
        /// Whether or not to render the resulting page fully or within the section
        /// </summary>
        public bool RenderErrorPageFully { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(bool renderErrorPageFully = false) : this(string.Empty, renderErrorPageFully)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(string errorMsg, bool renderErrorPageFully = false) : this(errorMsg, "Error", "Index", renderErrorPageFully)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(string errorMsg, string controllerName, string actionName, bool renderErrorPageFully = false)
            : this(new GeneralException(errorMsg, MultiTenancyFramework.ExceptionType.InvalidUserActionOrInput), controllerName, actionName, renderErrorPageFully)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(Exception ex, bool renderErrorPageFully = false)
            : this(ex, "Error", "Index", renderErrorPageFully)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(Exception ex, string controllerName, string actionName, bool renderErrorPageFully = false)
            : base(ex, controllerName, actionName)
        {
            var context = System.Web.HttpContext.Current;
            if (string.IsNullOrWhiteSpace(FromUrl))
            {
                FromUrl = context.Request.RawUrl;
            }
            RenderErrorPageFully = renderErrorPageFully;
            ExceptionType = ex.GetType();
            StackTrace = ex.StackTrace;

            if (ExceptionType == typeof(GeneralException))
            {
                ErrorType = (ex as GeneralException).ExceptionType;
            }
            else if (typeof(System.Data.Common.DbException).IsAssignableFrom(ExceptionType))
            {
                ErrorType = MultiTenancyFramework.ExceptionType.DatabaseRelated;
            }
            else
            {
                ErrorType = MultiTenancyFramework.ExceptionType.DoNothing;
            }
            if (string.IsNullOrWhiteSpace(StackTrace))
            {
                ErrorMessage = WebUtility.HtmlDecode(ex.GetFullExceptionMessage(context: context));
            }
            else
            {
                var nhBaseType = Type.ReflectionOnlyGetType("NHibernate.HibernateException, NHibernate", false, true);
                var isNHException = nhBaseType != null && nhBaseType.IsAssignableFrom(ExceptionType);
                if (isNHException)
                {
                    ErrorType = MultiTenancyFramework.ExceptionType.DatabaseRelated;
                    ErrorMessage = ex.Message;
                }
                else
                {
                    ErrorMessage = ex.GetFullExceptionMessage(context: context);
                }
            }
        }
    }
}
