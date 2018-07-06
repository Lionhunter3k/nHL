﻿using nHL.Web.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace nHL.Web.Filters
{
    public class NHibernateSession<T> : IAsyncActionFilter where T : ISessionWrapper
    {
        public NHibernateSession(T session)
        {
            this._session = session;
        }

        private readonly T _session;

        public async Task OnActionExecutionAsync(ActionExecutingContext context,
             ActionExecutionDelegate next)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
                return;
            _session.BeginTransaction();
            var executedContext = await next();
            if (_session.Transaction == null || !_session.Transaction.IsActive)
                return;
            if (executedContext.Exception != null)
            {
                await _session.Transaction.RollbackAsync();
            }
            else
            {
                await _session.Transaction.CommitAsync();
            }
        }
    }

}