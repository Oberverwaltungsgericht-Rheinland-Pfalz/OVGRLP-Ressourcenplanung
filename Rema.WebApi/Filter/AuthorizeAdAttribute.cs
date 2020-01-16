using Microsoft.AspNetCore.Mvc;

namespace Rema.WebApi.Filter
{
  public class AuthorizeAdAttribute : TypeFilterAttribute
  {
    public AuthorizeAdAttribute(string permission)
        : base(typeof(AuthorizeAdActionFilter))
    {
      Arguments = new object[] { permission };
    }
  }
}
