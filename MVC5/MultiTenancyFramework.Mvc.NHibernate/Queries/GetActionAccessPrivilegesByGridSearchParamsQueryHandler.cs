﻿using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using NHibernate.Criterion;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetActionAccessPrivilegesByGridSearchParamsQueryHandler : CoreGeneralWithGridPagingDAO<Privilege>, IDbQueryHandler<GetActionAccessPrivilegesByGridSearchParamsQuery, RetrievedData<ActionAccessPrivilege>>
    {
        public RetrievedData<ActionAccessPrivilege> Handle(GetActionAccessPrivilegesByGridSearchParamsQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<ActionAccessPrivilege>();
            if (!string.IsNullOrWhiteSpace(theQuery.Name))
            {
                query = query.Where(x => x.Name.IsInsensitiveLike(theQuery.Name) || x.DisplayName.IsInsensitiveLike(theQuery.Name));
            }
            if (theQuery.AccessScope.HasValue && theQuery.AccessScope.Value > 0)
            {
                query = query.Where(x => x.Scope == theQuery.AccessScope.Value);
            }
            return RetrieveUsingPaging(query, theQuery.PageIndex, theQuery.PageSize);
        }
    }
}