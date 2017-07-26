using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstellationExcelReports_POC
{
    class Agents
    {

        private int _agentKeyId;
        private string _agentId;
        private string _agentName;

        public Agents(int id, string agentId, string name)
        {
            _agentKeyId = id;
            _agentId = agentId;
            _agentName = name;
        }

        public int AgentKeyId
        {
            get { return _agentKeyId; }
        }
        public string AgentId
        {
            get { return _agentId; }
        }
        public string AgentName
        {
            get { return _agentName; }
        }
    }
}
