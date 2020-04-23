using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Sample_Programs
{
    public class PostAccountCreateChildURL : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                if (entity.LogicalName == "account")
                {
                    try
                    {
                        IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                        IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                        Guid childAccountId = entity.Id;
                        trace.Trace("Child Account ID: {0}", childAccountId);

                        Entity childAccountInfo = service.Retrieve("account", childAccountId, new ColumnSet(true));
                        if (childAccountInfo.Attributes.Contains("parentaccountid"))
                        {
                            Guid parentAccountId = ((EntityReference)entity["parentaccountid"]).Id;
                            trace.Trace("Parent Account ID: {0}", parentAccountId);
                            Entity parentAccountInfo = service.Retrieve("account", parentAccountId, new ColumnSet(true));
                            var hyperlink = string.Format("{0}{1}{2}{3}", "mypracticeinstance1.crm8.dynamics.com//main.aspx?pagetype=entityrecord&etn=", parentAccountInfo.LogicalName,"&id=", childAccountId);
                            parentAccountInfo.Attributes["test_childaccounturl"] = hyperlink;
                            service.Update(parentAccountInfo);

                        }

                    }
                    catch (Exception e)
                    {
                        trace.Trace("Create account: {0}", e.ToString());
                        throw;
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }
}
