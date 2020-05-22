using System.Threading.Tasks;
using Mashup.Api.Models;

namespace Mashup.Api.Interfaces
{
    public interface IMashupService
    {
        Task<MashupResultModel> BuildMashupModel(string mdId, string langCode);
    }
}
