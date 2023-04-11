using Common.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Module
{
    public class EFPControllerBase: ControllerBase
    {
        protected EFPapiResponse apiResponse { get; set; }

        protected readonly ILogger<dynamic> _logger;
    }
}
