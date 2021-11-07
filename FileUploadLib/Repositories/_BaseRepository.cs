using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using FileUploadLib.Data;
using Microsoft.AspNetCore.Http;
using FileUploadLib.Services;

namespace FileUploadLib.Repositories
{
    public class _BaseRepository<T, TKey>
        : IBaseRepository<T, TKey>, IEnumerable<T>
        where T : class, IBaseModel<TKey>
    {
        public FileLibContext _db;
        public UtilityService utilityService;
        private IHttpContextAccessor _httpContextAccessor;
        public _BaseRepository(FileLibContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            utilityService = new UtilityService(httpContextAccessor);
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual IEnumerable<T> GetElements()
        {
            return _db.Set<T>()
                .Where(e => !e.Deleted);
        }

        public virtual T GetElement(TKey id)
        {
            return GetElements().FirstOrDefault(e => e.Id.Equals(id));
        }

        public virtual async Task<T> GetElementAsync(TKey id)
        {
            return await _db.Set<T>().FirstOrDefaultAsync(e => e.Id.Equals(id) && !e.Deleted);
        }

        public static IEnumerable<T> GetFilteredElements(ICollection<T> elements)
        {
            return elements.Where(e => !e.Deleted);
        }

        public virtual bool Add(T element, bool saveChanges = true)
        {
            _db.Set<T>().Add(element);
            if (saveChanges)
            {
                return _db.SaveChanges() > 0;
            }
            return true;
        }

        public virtual async Task<bool> AddAsync(T element, bool saveChanges = true)
        {
            await _db.Set<T>().AddAsync(element);
            if (saveChanges)
            {
                return await _db.SaveChangesAsync() > 0;
            }
            return true;
        }

        public virtual bool AddRange(IEnumerable<T> elements)
        {
            _db.Set<T>().AddRange(elements);
            return _db.SaveChanges() > 0;
        }

        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> elements)
        {
            await _db.Set<T>().AddRangeAsync(elements);
            return await _db.SaveChangesAsync() > 0;
        }

        public virtual bool Update(T element, bool saveChanges = true)
        {
            _db.Entry(element).State = EntityState.Modified;
            _db.Entry(element).Property(p => p.CreatedDate).IsModified = false;
            _db.Entry(element).Property(p => p.CreatedBy).IsModified = false;
            _db.Entry(element).Property(p => p.Deleted).IsModified = false;
            if (saveChanges)
            {
                return _db.SaveChanges() > 0;
            }
            return true;
        }

        public virtual async Task<bool> UpdateAsync(T element, bool saveChanges = true)
        {
            _db.Entry(element).State = EntityState.Modified;
            _db.Entry(element).Property(p => p.CreatedDate).IsModified = false;
            _db.Entry(element).Property(p => p.CreatedBy).IsModified = false;
            _db.Entry(element).Property(p => p.Deleted).IsModified = false;
            if (saveChanges)
            {
                return await _db.SaveChangesAsync() > 0;
            }
            return true;
        }

        public virtual bool Delete(T element)
        {
            element.Deleted = true;
            _db.Entry(element).State = EntityState.Modified;
            _db.Entry(element).Property(p => p.CreatedDate).IsModified = false;
            _db.Entry(element).Property(p => p.CreatedBy).IsModified = false;
            return _db.SaveChanges() > 0;
        }

        public virtual bool DeleteRange(IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                element.Deleted = true;
                _db.Entry(element).State = EntityState.Modified;
                _db.Entry(element).Property(p => p.CreatedDate).IsModified = false;
                _db.Entry(element).Property(p => p.CreatedBy).IsModified = false;
            }
            return _db.SaveChanges() > 0;
        }

        public virtual async Task<bool> DeleteAsync(T element)
        {
            element.Deleted = true;
            _db.Entry(element).State = EntityState.Modified;
            _db.Entry(element).Property(p => p.CreatedDate).IsModified = false;
            _db.Entry(element).Property(p => p.CreatedBy).IsModified = false;
            return await _db.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteRangeAsync(IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                element.Deleted = true;
                _db.Entry(element).State = EntityState.Modified;
                _db.Entry(element).Property(p => p.CreatedDate).IsModified = false;
                _db.Entry(element).Property(p => p.CreatedBy).IsModified = false;
            }
            return await _db.SaveChangesAsync() > 0;
        }

        public virtual bool Delete(TKey id)
        {
            T element = GetElement(id);
            return Delete(element);
        }

        public virtual async Task<bool> DeleteAsync(TKey id)
        {
            T element = GetElement(id);
            return await DeleteAsync(element);
        }

        public virtual bool Exists(TKey id)
        {
            T element = GetElement(id);
            return element != null;
        }

        public virtual async Task<bool> ExistsAsync(TKey id)
        {
            T element = await GetElementAsync(id);
            return element != null;
        }

        public virtual bool SaveChanges()
        {
            return _db.SaveChanges() > 0;
        }

        public virtual async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetElements().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class _BaseRepository<T> : _BaseRepository<T, int>, IBaseRepository<T, int>
        where T : class, IBaseModel<int>
    {
        public _BaseRepository(FileLibContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }
    }

    public interface IBaseRepository<T, TKey>
    {
        IEnumerable<T> GetElements();
        T GetElement(TKey id);
    }
}
