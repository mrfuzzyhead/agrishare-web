using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public partial class SupplierService : IEntity
    {
        public static string DefaultSort = "Title";

        private File _photo;
        public File Photo
        {
            get
            {
                if (_photo == null && !PhotoPath.IsEmpty())
                    _photo = new File(PhotoPath);
                return _photo;
            }
            set
            {
                _photo = value;
                if (_photo != null)
                    PhotoPath = _photo.Name + _photo.Extension;
            }
        }

        public static SupplierService Find(int Id = 0)
        {
            if (Id == 0)
                return new SupplierService
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.SupplierServices.Include(o => o.Supplier).Where(o => !o.Deleted && !o.Supplier.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<SupplierService> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", int SupplierId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.SupplierServices.Include(o => o.Supplier).Where(o => !o.Deleted && !o.Supplier.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (SupplierId > 0)
                    query = query.Where(o => o.SupplierId == SupplierId);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", int SupplierId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.SupplierServices.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (SupplierId > 0)
                    query = query.Where(o => o.SupplierId == SupplierId);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var supplier = Supplier;
            if (supplier != null)
                SupplierId = supplier.Id;
            Supplier = null;

            var photo = Photo;
            if (photo != null) PhotoPath = photo.Name + photo.Extension;
            Photo = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Photo = photo;
            Supplier = supplier;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.SupplierServices.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.SupplierServices.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete()
        {
            if (Id == 0)
                return false;

            Deleted = true;
            return Update();
        }

        public object Json()
        {
            return new
            {
                Id,
                Title,
                Photo = Photo?.JSON(),
                Supplier = Supplier?.Json(),
                Description,
                Cost,
                Stock,
                DateCreated,
                LastModified
            };
        }
    }
}
