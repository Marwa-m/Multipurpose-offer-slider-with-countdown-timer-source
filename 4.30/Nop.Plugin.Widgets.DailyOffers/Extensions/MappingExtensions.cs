using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using Nop.Plugin.Widgets.DailyOffers.Models;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Extensions
{
   public static class MappingExtensions
    {
        public static TModel ToModel<TEntity, TModel>(this TEntity entity, TModel model)
            where TEntity : BaseEntity where TModel : BaseNopEntityModel
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return entity.MapTo(model);
        }
        #region Offer
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        public static OfferModel ToModel(this Offer entity)
        {
            return entity.MapTo<Offer, OfferModel>();
        }

        public static Offer ToEntity(this OfferModel model)
        {
            return model.MapTo<OfferModel, Offer>();
        }

        public static Offer ToEntity(this OfferModel model, Offer destination)
        {
            return model.MapTo(destination);
        }
        
        public static ProductModel ToModel(this Product model, ProductModel destination)
        {
            return model.MapTo(destination);
        }
        public static Product ToEntity(this ProductModel model, Product destination)
        {
            return model.MapTo(destination);
        }

        #endregion
    }
}
