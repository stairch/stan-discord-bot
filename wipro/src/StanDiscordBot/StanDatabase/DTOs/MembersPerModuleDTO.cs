namespace StanDatabase.DTOs
{
    public class MembersPerModuleDTO
    {
        private string _moduleName;
        private int _memberCount;

        public string ModuleName { 
            get { return _moduleName; } 
            set { _moduleName = value; }
        }
        public int MemberCount { 
            get { return _memberCount; } 
            set { _memberCount = value; }
        }
    }
}
