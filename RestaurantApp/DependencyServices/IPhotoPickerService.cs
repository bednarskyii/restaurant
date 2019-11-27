using System;
using System.IO;
using System.Threading.Tasks;

namespace RestaurantApp.DependencyServices
{
    public interface IPhotoPickerService
    {
        Task<Stream> GetImageStreamAsync();
    }
}
