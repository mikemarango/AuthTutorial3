using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Mvc.Models
{
  public class OrderFrameViewModel
  {
    public OrderFrameViewModel(string address)
    {
      Address = address;
    }

    public string Address { get; private set; } = string.Empty;
  }
}
