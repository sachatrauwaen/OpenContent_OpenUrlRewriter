using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DotNetNuke.Common.Utilities;

using DotNetNuke.Entities.Modules;
using Satrabel.HttpModules.Provider;
using DotNetNuke.Framework.Providers;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using Satrabel.OpenContent.Components;
using Satrabel.OpenContent.Components.Handlebars;
using Satrabel.OpenContent.Components.Json;
using Satrabel.OpenContent.Components.Manifest;
using Satrabel.OpenContent.Components.Datasource;
using Satrabel.OpenContent.Components.UrlRewriter;

namespace Satrabel.OpenUrlRewriter.OpenContent
{
    public class OpenContentUrlRuleProvider : UrlRuleProvider
    {

        private const string ProviderType = "urlRule";
        private const string ProviderName = "openContentUrlRuleProvider";

        private readonly ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private readonly bool includePageName = true;

        public OpenContentUrlRuleProvider()
        {
            /*
            var objProvider = (DotNetNuke.Framework.Providers.Provider)_providerConfiguration.Providers[ProviderName];
            if (!String.IsNullOrEmpty(objProvider.Attributes["includePageName"]))
            {
                includePageName = bool.Parse(objProvider.Attributes["includePageName"]);
            }
             */
            //CacheKeys = new string[] { "PropertyAgent-ProperyValues-All" };

            //HelpUrl = "https://openurlrewriter.codeplex.com/wikipage?title=OpenContent";
        }

        public override List<UrlRule> GetRules(int PortalId)
        {
            var rules = OpenContentUrlProvider.GetRules(PortalId);

            return rules.Select(r => new UrlRule() {
                CultureCode = r.CultureCode,
                TabId = r.TabId,
                RuleType = UrlRuleType.Module,
                Parameters = r.Parameters,
                Action = UrlRuleAction.Rewrite,
                Url = CleanupUrl(r.Url),
                RemoveTab = !includePageName
            }).ToList();

            /*
            Dictionary<string, Locale> dicLocales = LocaleController.Instance.GetLocales(PortalId);  
            List<UrlRule> Rules = new List<UrlRule>();
            OpenContentController occ = new OpenContentController();

            ModuleController mc = new ModuleController();
            ArrayList modules = mc.GetModulesByDefinition(PortalId, "OpenContent");
            //foreach (ModuleInfo module in modules.OfType<ModuleInfo>().GroupBy(m => m.ModuleID).Select(g => g.First())){                
            foreach (ModuleInfo module in modules.OfType<ModuleInfo>())
            {
                OpenContentSettings settings = new OpenContentSettings(module.ModuleSettings);
                int MainTabId = settings.DetailTabId > 0 ? settings.DetailTabId : (settings.TabId > 0 ? settings.TabId : module.TabID);
                int MainModuleId = settings.IsOtherModule ? settings.ModuleId : module.ModuleID;

                if (settings.Template != null && settings.Template.IsListTemplate && ( !settings.IsOtherModule || settings.DetailTabId > 0) )
                {
                    var physicalTemplateFolder = settings.TemplateDir.PhysicalFullDirectory+ "\\";

                    HandlebarsEngine hbEngine = new HandlebarsEngine();
                    if (!string.IsNullOrEmpty(settings.Manifest.DetailUrl))
                    {
                        hbEngine.Compile(settings.Manifest.DetailUrl);
                    }
                    foreach (KeyValuePair<string, Locale> key in dicLocales)
                    {
                        string CultureCode = key.Value.Code;
                        string RuleCultureCode = (dicLocales.Count > 1 ? CultureCode : null);

                        //var contents = occ.GetContents(MainModuleId);
                        var ds = DataSourceManager.GetDataSource(settings.Manifest.DataSource);            
                        var dsContext = new DataSourceContext()
                        {
                            ModuleId = MainModuleId,
                            TemplateFolder = settings.TemplateDir.FolderPath,
                            Config = settings.Manifest.DataSourceConfig
                        };
                        IEnumerable<IDataItem> dataList = new List<IDataItem>();
                        dataList = ds.GetAll(dsContext, null).Items;



                        if (dataList.Count() > 1000)
                        {
                            continue;
                        }
                        foreach (IDataItem content in dataList)
                        {
                            
                            string url = "content-" + content.Id;
                            if (!string.IsNullOrEmpty(settings.Manifest.DetailUrl))
                            {
                                //string dataJson = content.Data;
                                try
                                {
              
                                    ModelFactory mf = new ModelFactory(content, settings.Data, physicalTemplateFolder, settings.Template.Manifest, settings.Template, settings.Template.Main, module, PortalId, CultureCode, MainTabId, MainModuleId);
                                    dynamic model = mf.GetModelAsDynamic(true);

                                    url = CleanupUrl(hbEngine.Execute(model));
                                    //title = OpenContentUtils.CleanupUrl(dyn.Title);
                                }
                                catch (Exception ex)
                                {
                                    Log.Logger.Error("Failed to generate url for opencontent item " + content.Id, ex);
                                }
                            }

                            if (!string.IsNullOrEmpty(url))
                            {
                                var rule = new UrlRule
                                {
                                    CultureCode = RuleCultureCode,
                                    TabId = MainTabId,
                                    RuleType = UrlRuleType.Module,
                                    Parameters = "id=" + content.Id,
                                    Action = UrlRuleAction.Rewrite,
                                    Url = CleanupUrl(url),
                                    RemoveTab = !includePageName
                                };
                                if (Rules.Any(r => r.Url == rule.Url && r.CultureCode == rule.CultureCode && r.TabId == rule.TabId))
                                {
                                    rule.Url = content.Id + "-" + CleanupUrl(url);
                                }
                                bool RuleExist = Rules.Any(r => r.RuleType == rule.RuleType && r.CultureCode == rule.CultureCode && r.TabId == rule.TabId && r.Parameters == rule.Parameters && r.Action == rule.Action);
                                
                                if (!RuleExist) Rules.Add(rule);

                            }
                        }
                    }
                }
            }
            return Rules;
             */
        }

    }
}