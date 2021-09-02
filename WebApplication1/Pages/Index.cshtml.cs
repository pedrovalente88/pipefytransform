using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string JsonReturn = null;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            string dir = "Files";
            string file = "pipefy.json";
            string path = string.Format("{0}/{1}", dir, file);

            string result = System.IO.File.ReadAllText(path);
            var json = (dynamic)JsonConvert.DeserializeObject(result);
            var data = json.data;
            var table_records = data.table_records;
            var edges = table_records.edges;

            var nodeList = new List<node>();

            foreach(var edge in edges)
            {
                var edgeNode = edge.node;

                var node = new node { id = edgeNode.id };

                var recordFieldList = new List<record_field>();
                foreach(var rf in edgeNode.record_fields)
                {
                    var record_field = new record_field
                    {
                        name = rf.name,
                        value = rf.value
                    };
                    recordFieldList.Add(record_field);
                }

                node.record_fields = recordFieldList;
                nodeList.Add(node);
            }

            var names = nodeList.SelectMany(x => x.record_fields).Select(y => y.name).Distinct();

            var dictionaryList = new List<Dictionary<string, string>>(); 
            
            foreach(var node in nodeList)
            {
                var keyValueList = new List<KeyValuePair<string, string>>();

                var keyValueId = new KeyValuePair<string,string>("id", node.id);
                keyValueList.Add(keyValueId);

                foreach (var name in names)
                {
                    var recordField = node.record_fields.FirstOrDefault(x => x.name == name);
                    if(recordField != null)
                    {
                        var keyValueName = new KeyValuePair<string, string>(name, recordField.value);
                        keyValueList.Add(keyValueName);
                    }
                }
                
                dictionaryList.Add(new Dictionary<string, string>(keyValueList));                
            }

            JsonReturn = JsonConvert.SerializeObject(dictionaryList);
        }
    }
}
