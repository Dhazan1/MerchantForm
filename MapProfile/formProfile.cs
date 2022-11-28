using AutoMapper;
using ContextFormData;
using MerchantForm.Model;
using MerchantForm.Models.WebMarchant;

namespace MerchantForm.MapProfile
{
    public class formProfile : Profile

    {
        public formProfile()
        {
            CreateMap<MerchantFormDto, FormDetail>().ReverseMap();
            CreateMap<AcctResponseRequired,AcctResponse>().ReverseMap();
        }
    }
}
