using System;
using System.Linq;

namespace Exchange.Rates.Core.Extensions;

public static class UriExtensions
{
  public static Uri Append(this Uri uri, params string[] paths)
  {
    return new(paths.Aggregate(uri.AbsoluteUri, (current, path) =>
      $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
  }
}