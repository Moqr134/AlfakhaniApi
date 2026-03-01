using SecondApi.Domin.Sizes;
using SherdProject.DTO;

namespace SecondApi.App.ISirvice
{
    public interface ISizesService
    {
        public Task<bool> AddSize(SizeDto sizeDto);
        public Task<bool> UpdateSize(SizeDto sizeDto);
        public Task<string> DeleteSize(SizeDto dto);
        public Task<SizesEntity> GetSize(string name,int ItemId);
    }
}
