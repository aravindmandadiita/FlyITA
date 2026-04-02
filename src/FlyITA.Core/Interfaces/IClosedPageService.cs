using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface IClosedPageService
{
    ClosedPageModel GetClosedMessage(string currentPage);
}
