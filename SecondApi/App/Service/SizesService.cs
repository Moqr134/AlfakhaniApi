using Infrastructure.ORM;
using SecondApi.App.ISirvice;
using SecondApi.Domin.Sizes;
using SecondApi.Infrastructure.Services;
using SherdProject.DTO;

namespace SecondApi.App.Service
{
    public class SizesService(DBContext context) : MasterService(context) , ISizesService
    {
        public async Task<bool> AddSize(SizeDto sizeDto)
        {
            try
            {
                SizesEntity sizesEntity = new()
                {
                    Name = sizeDto.Name,
                    Price = sizeDto.Price,
                    ItemId = sizeDto.ItemId,
                    CreateDate = DateTime.UtcNow.AddHours(3),
                    CreateUserId =sizeDto.CreateBy
                };
                await Context.Sizes.AddAsync(sizesEntity);
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "SizesService=>AddSize");
                throw;
            }
        }

        public async Task<string> DeleteSize(SizeDto dto)
        {
            try
            {
                SizesEntity? sizesEntity = Context.Sizes.Where(s=>s.Name==dto.Name&&s.ItemId==dto.ItemId).FirstOrDefault();
                if (sizesEntity == null)
                {
                    return "الحجم غير موجود لحذفه";
                }
                Context.Sizes.Remove(sizesEntity);
                await Context.SaveChangesAsync();
                return "true";
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "SizesService=>DeleteSize");
                throw;
            }
        }

        public async Task<SizesEntity> GetSize(string name,int itemId)
        {
            try
            {
                return Context.Sizes.Where(x => x.Name == name && x.ItemId==itemId).FirstOrDefault();
            }
            catch(Exception ex)
            {
                await loger.WriteAsync(ex, "SizesService=>GetSize");
                throw;
            }
        }

        public async Task<bool> UpdateSize(SizeDto sizeDto)
        {
            try
            {
                SizesEntity? sizesEntity = Context.Sizes.Find(sizeDto.Id);
                if (sizesEntity == null)
                {
                    return false;
                }
                sizesEntity.Price = sizeDto.Price;
                sizesEntity.Name = sizeDto.Name;
                sizesEntity.ItemId = sizeDto.ItemId;
                sizesEntity.UpdateDate = DateTime.UtcNow.AddHours(3);
                sizesEntity.UpdateUserId = sizeDto.CreateBy;
                Context.Sizes.Entry(sizesEntity);
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "SizesService=>DeleteSize");
                throw;
            }
        }
    }
}
