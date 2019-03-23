using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microex.All.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microex.All.EntityFramework
{
    public class UnitOfWork<TDbContext,TUser> where TDbContext : IntegratedDbContext<TUser> where TUser : IdentityUser
    {
        private IMediator _mediator;
        //private bool _isSaving;
        public ConcurrentQueue<IRequest<bool>> DomainEvents { get; } = new ConcurrentQueue<IRequest<bool>>();
        public TDbContext DbContext { get; private set; }
        public UnitOfWork(TDbContext dbContext,IMediator mediator)
        {
            _mediator = mediator;
            DbContext = dbContext;
        }
        
        public async Task<bool> SaveChangesAsync()
        {
            while (DomainEvents.TryDequeue(out var domainEvent))
            {
                if (await _mediator.Send(domainEvent))
                {
                    continue;
                }
                // bug: 日志记录
                //throw new Exception("domain event failed." + domainEvent.ToJson());
                //任意领域事件失败均移除后续领域事件
                this.ClearDomainEvent();
                return false;
            }
            //while (this._isSaving)
            //{
            //    await Task.Delay(100);
            //}

            //this._isSaving = true;
            try
            {
                this.DbContext.UowSaveChanges();
                //this._isSaving = false;
                return true;
            }
            catch (Exception)
            {
                // bug: 日志记录
                this.ClearDomainEvent();
                //this._isSaving = false;
                throw;
            }
        }

        public void ClearDomainEvent()
        {
            while (this.DomainEvents.TryDequeue(out _))
            {
            }
        }
    }
}