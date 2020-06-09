using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.DailyOffers.Domain.Offers;
using Nop.Plugin.Widgets.DailyOffers.Models.Offers;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Domain.Infrastructure.Mapper
{
    class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public AdminMapperConfiguration()
        {

            //offer
            CreateMap<Offer, OfferModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            CreateMap<OfferModel, Offer>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());

            CreateMap<ProductOffer, OfferProductModel>()
                .ForMember(model => model.ProductName, options => options.Ignore());
            CreateMap<OfferProductModel, ProductOffer>()
                .ForMember(entity => entity.OfferId, options => options.Ignore())
                .ForMember(entity => entity.ProductId, options => options.Ignore())
                .ForMember(entity => entity.Offer, options => options.Ignore());
            //  .ForMember(entity => entity.Product, options => options.Ignore());
            CreateMap<OfferWidgetZone, OfferWidgetZoneModel>();
            CreateMap<OfferWidgetZoneModel, OfferWidgetZone>()
                .ForMember(entity => entity.OfferId, options => options.Ignore())
                .ForMember(entity => entity.Offer, options => options.Ignore());
        }

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

    }
}
