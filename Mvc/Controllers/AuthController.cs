﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mvc.Controllers
{
  public class AuthController : Controller
  {
    public IActionResult AccessDenied()
    {
      return View();
    }
  }
}
