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
        Task<IEnumerable<Drug>> SearchByNameAsync(string name);
        Task<IEnumerable<Drug>> SearchByIngredientAsync(string ingredient);
        Task<IEnumerable<Drug>> FilterByCompanyAsync(string companyName);
        Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> FilterByCategoryAsync(string category, int page, int pageSize);
        Task<IEnumerable<Drug>> GetRelatedByIngredientAsync(string id);
        Task<IEnumerable<Drug>> GetRelatedByCompanyAsync(string id);
        Task<IEnumerable<Drug>> GetTopSearchedDrugsAsync();
        Task IncrementSearchCountAsync(string id);
        Task<bool> UpdateDrugAsync(string id, UpdateDrugModelView updatedDrug);
        Task<IEnumerable<DrugModelView>> GetTopNewRegisteredDrugsAsync();
        Task<IEnumerable<DrugModelView>> GetTopWithdrawnDrugsAsync();
        Task<IEnumerable<object>> GetTopCompaniesByDrugsAsync();
        Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> FilterByDrugGroupAsync(string group, int page, int pageSize);

    }
}
