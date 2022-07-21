﻿using ActionEngine.DataClass.Model;
using Dapr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEngine.Module
{
    public class ExcutionModule
    {
        private readonly DaprClient _daprClient;

        public ExcutionModule(DaprClient daprClient)
        {
            _daprClient = daprClient;
            IExection = new Dictionary<EnumActionType, ExectionMethod>()
            {
                {EnumActionType.ApiGet, new ExectionMethod(new API(_daprClient).ApiGet) }
            };
        }

        private Dictionary<EnumActionType, ExectionMethod> IExection;

        private delegate Dictionary<string, object> ExectionMethod(ExecuteAction action, Dictionary<string, object> inModel);


        public Dictionary<string, object> Go(ExecuteAction action, Dictionary<string, object> inModel)
        {
            return new ExectionMethod(IExection[action.ActionType]).Invoke(action, inModel);
        }
    }
}
