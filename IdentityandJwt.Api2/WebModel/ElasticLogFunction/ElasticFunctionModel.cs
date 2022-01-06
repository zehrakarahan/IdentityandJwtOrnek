using ElasticsearchCRUD;
using ElasticsearchCRUD.Tracing;
using IdentityandJwt.Api2.WebModel.ElasticSearchModel;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2.WebModel.ElasticLogFunction
{
    public class ElasticFunctionModel
    {
        public static ConnectionSettings Connectionsettings(int tableno)
        {
            ConnectionSettings connSettings = new ConnectionSettings(new Uri("http://localhost:9200/"));
            switch (tableno)
            {
                case 1:
                    connSettings.DefaultIndex("PortalLogs")
                    .DefaultMappingFor<PortalLogs>(m => m
                    .IndexName("PortalLogs"));
                    return connSettings;
            
                default: return null;
            };


        }
        public static void ListData(List<PortalLogs> model, int tableno)
        {
            switch (tableno)
            {
                case 1:
                    ConnectionSettings connSettingsin = Connectionsettings(tableno);
                    ElasticClient elasticClientin = new ElasticClient(connSettingsin);

                    var bulkIndexerin = new BulkDescriptor();
                    foreach (var document in model.ToList())
                    {
                        bulkIndexerin.Index<PortalLogs>(i => i
                       .Document(document)
                       .Id(document.GuidNoElastic)
                       .Index("PortalLogs"));
                    }
                    elasticClientin.Bulk(bulkIndexerin);
                    break;
              
       
            }
        }
    }
}
