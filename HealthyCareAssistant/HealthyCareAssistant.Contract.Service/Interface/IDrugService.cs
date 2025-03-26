using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.ModelViews.DrugModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IDrugService
    {
        Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> GetAllDrugsPaginatedAsync(int page, int pageSize);
        Task<Drug> GetDrugByIdAsync(string id);
        Task<string> CreateDrugAsync(DrugModelView drug);
        Task<bool> DeleteDrugAsync(string id);
        Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> SearchDrugsAsync(DrugSearchRequest request, int page, int pageSize);
        Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> FilterDrugsAsync(DrugFilterRequest request, int page, int pageSize);
        Task<IEnumerable<Drug>> GetRelatedDrugsAsync(string id, string type);  // ingredient | company
        Task IncrementSearchCountAsync(string id);
        Task<bool> UpdateDrugAsync(string id, UpdateDrugModelView updatedDrug);
        Task<IEnumerable<DrugModelView>> GetTopDrugsByTypeAsync(string type); // new | withdrawn | searched
        Task<IEnumerable<object>> GetTopCompaniesByDrugsAsync();
        Task<IEnumerable<string>> GetAllCompaniesAsync();
    }
}
